using Microsoft.WindowsAzure.Mobile.Service;  

namespace homesecurityService.DataObjects
{
    public class Account : EntityData
    {
        public string Email { get; set; }
        public byte[] Salt { get; set; }
        public byte[] SaltedAndHashedPassword { get; set; }
        public bool Verified { get; set; }
    }
}