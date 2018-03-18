using System.Collections.Generic;

namespace minecraft_launcher_v2.Serialization
{
    public class RootError
    {
        public string error { get; set; }
        public string errorMessage { get; set; }
        public string cause { get; set; }
    }

    public class Agent
    {
        public string name { get; set; }
        public int version { get; set; }
    }

    public class RootAuthenticationRequest
    {
        public Agent agent { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string clientToken { get; set; }
        public bool requestUser { get; set; }
    }



    public class AvailableProfile
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool legacy { get; set; }
    }

    public class SelectedProfile
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool legacy { get; set; }
    }

    public class RootAuthenticationResponse
    {
        public string accessToken { get; set; }
        public string clientToken { get; set; }
        public User user { get; set; }
        public List<AvailableProfile> availableProfiles { get; set; }
        public SelectedProfile selectedProfile { get; set; }
    }



    public class SelectedProfileRefresh
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Property
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public List<Property> properties { get; set; }
    }

    public class RootRefreshRequestResponse
    {
        public string accessToken { get; set; }
        public string clientToken { get; set; }
        public SelectedProfileRefresh selectedProfile { get; set; }
        public User user { get; set; }
        public bool requestUser { get; set; }
    }



    public class RootSignOutRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }



    public class RootValidateInvalidateRequest
    {
        public string accessToken { get; set; }
        public string clientToken { get; set; }
    }

}
