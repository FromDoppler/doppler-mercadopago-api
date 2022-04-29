#!/bin/sh

commit=""
name=""
version=""
versionPre=""
imageName=""

print_help () {
    echo ""
    echo "Usage: sh build-n-publish.sh [OPTIONS]"
    echo ""
    echo "Build project's docker images and publish them to DockerHub"
    echo ""
    echo "Options:"
    echo "  -i, --image, image name (mandatory)"
    echo "  -c, --commit (mandatory)"
    echo "  -n, --name, version name"
    echo "  -v, --version, version number"
    echo "  -s, --pre-version-suffix (optional, only with version)"
    echo "  -h, --help"
    echo "Only one of name or version parameters is required, and cannot be included together."
    echo
    echo "Examples:"
    echo "  sh build-n-publish.sh --image=dopplerdock/mercadopago-api --commit=aee25c286a7c8265e2b32ccc293f5ab0bd7a9d57 --version=v1.2.11"
    echo "  sh build-n-publish.sh --image=dopplerdock/mercadopago-api --commit=e247ba0527665eb9dd7ffbff00bb42e5073cd457 --version=v0.0.0 --pre-version-suffix=commit-e247ba0527665eb9dd7ffbff00bb42e5073cd457"
    echo "  sh build-n-publish.sh -i=dopplerdock/mercadopago-api -c=94f85efb9c3689f409104ef7cde6813652ca59fb -v=v12.34.5"
    echo "  sh build-n-publish.sh -i=dopplerdock/mercadopago-api -c=94f85efb9c3689f409104ef7cde6813652ca59fb -v=v12.34.5 -s=beta1"
    echo "  sh build-n-publish.sh -i=dopplerdock/mercadopago-api -c=94f85efb9c3689f409104ef7cde6813652ca59fb -v=v12.34.5 -s=pr123"
}

for i in "$@" ; do
case $i in
    -i=*|--image=*)
    imageName="${i#*=}"
    ;;
    -c=*|--commit=*)
    commit="${i#*=}"
    ;;
    -n=*|--name=*)
    name="${i#*=}"
    ;;
    -v=*|--version=*)
    version="${i#*=}"
    ;;
    -s=*|--pre-version-suffix=*)
    versionPre="${i#*=}"
    ;;
    -h|--help)
    print_help
    exit 0
    ;;
esac
done

if [ -z "${imageName}" ]
then
  echo "Error: image parameter is mandatory"
  print_help
  exit 1
fi

if [ -z "${commit}" ]
then
  echo "Error: commit parameter is mandatory"
  print_help
  exit 1
fi

if [ -n "${version}" ] && [ -n "${name}" ]
then
  echo "Error: name and version parameters cannot be included together"
  print_help
  exit 1
fi

if [ -z "${version}" ] && [ -z "${name}" ]
then
  echo "Error: one of name or version parameters is required"
  print_help
  exit 1
fi

if [ -z "${version}" ] && [ -n "${versionPre}" ]
then
  echo "Error: pre-version-suffix parameter is only accepted along with version parameter"
  print_help
  exit 1
fi

# TODO: validate commit format
# TODO: validate version format (if it is included)

# Stop script on NZEC
set -e
# Stop script if unbound variable found (use ${var:-} if intentional)
set -u

# Lines added to get the script running in the script path shell context
# reference: http://www.ostricher.com/2014/10/the-right-way-to-get-the-directory-of-a-bash-script/
cd "$(dirname "$0")"

# To avoid issues with MINGW and Git Bash, see:
# https://github.com/docker/toolbox/issues/673
# https://gist.github.com/borekb/cb1536a3685ca6fc0ad9a028e6a959e3
export MSYS_NO_PATHCONV=1
export MSYS2_ARG_CONV_EXCL="*"

if [ -n "${version}" ]
then
  versionBuild=${commit}
  # Ugly code to deal with versions
  # Input:
  #   version=v12.34.5
  #   versionBuild=94f85efb9c
  #   versionPre=0pr
  # Output:
  #   versionMayor=pre-v12
  #   versionMayorMinor=pre-v12.34
  #   versionMayorMinorPatch=pre-v12.34.5
  #   versionMayorMinorPatchPre=pre-v12.34.5-0pr
  #   versionFull=pre-v12.34.5-0pr+94f85efb9c
  #   versionFullForTag=pre-v12.34.5-0pr_94f85efb9c
  # region Ugly code to deal with versions

  versionFull=${version}

  if [ -n "${versionPre}" ]
  then
    versionFull=${versionFull}-${versionPre}
  fi

  if [ -n "${versionBuild}" ]
  then
    versionFull=${versionFull}+${versionBuild}
  fi

  # https://semver.org/spec/v2.0.0.html#backusnaur-form-grammar-for-valid-semver-versions
  # <valid semver> ::= <version core>
  #                  | <version core> "-" <pre-release>
  #                  | <version core> "+" <build>
  #                  | <version core> "-" <pre-release> "+" <build>
  #
  # <version core> ::= <major> "." <minor> "." <patch>
  versionBuild="$(echo "${versionFull}"+ | cut -d'+' -f2)"
  versionMayorMinorPatchPre="$(echo "${versionFull}" | cut -d'+' -f1)" # v0.0.0-xxx (ignoring `+` if exists)
  versionPre="$(echo "${versionMayorMinorPatchPre}"- | cut -d'-' -f2)"
  versionMayorMinorPatch="$(echo "${versionMayorMinorPatchPre}" | cut -d'-' -f1)" # v0.0.0 (ignoring `-` if exists)
  versionMayor="$(echo "${versionMayorMinorPatch}" | cut -d'.' -f1)" # v0
  versionMinor="$(echo "${versionMayorMinorPatch}" | cut -d'.' -f2)"
  versionMayorMinor="${versionMayor}.${versionMinor}" # v0.0
  # by the moment we do not need it, versionPatch only for demo purposes
  # versionPatch="$(echo "${versionMayorMinorPatch}" | cut -d'.' -f3)"

  if [ -z "${versionBuild}" ]
  then
    canonicalTag=${versionMayorMinorPatchPre}
  else
    # because `+` is not accepted in tag names
    canonicalTag=${versionMayorMinorPatchPre}_${versionBuild}
  fi

  if [ -n "${versionPre}" ]
  then
    preReleasePrefix="pre-"
    versionMayor=${preReleasePrefix}${versionMayor}
    versionMayorMinor=${preReleasePrefix}${versionMayorMinor}
    versionMayorMinorPatch=${preReleasePrefix}${versionMayorMinorPatch}
    versionMayorMinorPatchPre=${preReleasePrefix}${versionMayorMinorPatchPre}
    versionFull=${preReleasePrefix}${versionFull}
    canonicalTag=${preReleasePrefix}${canonicalTag}
  fi
  # endregion Ugly code to deal with versions
fi

if [ -n "${name}" ]
then
  versionFull=${name}-${commit}
  canonicalTag=${versionFull}
fi

docker build \
    -t "${imageName}:${canonicalTag}" \
    --build-arg version="${imageName}:${canonicalTag}" \
    .

if [ -n "${version}" ]
then
    docker tag "${imageName}:${canonicalTag}" "${imageName}:${versionMayor}"
    docker tag "${imageName}:${canonicalTag}" "${imageName}:${versionMayorMinor}"
    docker tag "${imageName}:${canonicalTag}" "${imageName}:${versionMayorMinorPatch}"
    docker tag "${imageName}:${canonicalTag}" "${imageName}:${versionMayorMinorPatchPre}"

    docker push "${imageName}:${canonicalTag}"
    docker push "${imageName}:${versionMayorMinorPatchPre}"
    docker push "${imageName}:${versionMayorMinorPatch}"
    docker push "${imageName}:${versionMayorMinor}"
    docker push "${imageName}:${versionMayor}"
fi

if [ -n "${name}" ]
then
    docker tag "${imageName}:${canonicalTag}" "${imageName}:${name}"

    docker push "${imageName}:${canonicalTag}"
    docker push "${imageName}:${name}"
fi
