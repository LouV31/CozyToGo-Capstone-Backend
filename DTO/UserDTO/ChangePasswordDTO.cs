﻿namespace CozyToGo.DTO.UserDTO
{
    public class ChangePasswordDTO
    {
        public int IdUser { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
