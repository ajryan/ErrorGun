using System;

namespace ErrorGun.Web.Models
{
    public class AdminModel
    {
        public string Message { get; set; }
        public string ServerBitness { get; set; }
        public string ProcessBitness { get; set; }

        public AdminModel(bool passwordCorrect)
        {
            Message = passwordCorrect ? "Password correct" : "Password incorrect";
            if (!passwordCorrect)
                return;

            ServerBitness = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            ProcessBitness = Environment.Is64BitProcess ? "x64" : "x86";
        }
    }
}