namespace Doppler.HelloMicroservice.DopplerSecurity
{
    public static class Policies
    {
        public const string ONLY_SUPERUSER = nameof(ONLY_SUPERUSER);
        public const string OWN_RESOURCE_OR_SUPERUSER = nameof(OWN_RESOURCE_OR_SUPERUSER);
    }
}
