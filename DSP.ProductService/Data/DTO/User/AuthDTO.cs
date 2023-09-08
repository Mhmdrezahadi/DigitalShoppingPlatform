﻿using System.ComponentModel.DataAnnotations;

namespace DSP.ProductService.Data
{
    /// <summary>
    /// کاربر جهت ورود
    /// </summary>
    public class AuthDTO
    {
        /// <summary>
        /// شماره تلفن
        /// </summary>
        [StringLength(11)]
        [Required]
        public string MobileNumber { get; set; }

        /// <summary>
        /// کد تایید
        /// </summary>
        [Required]
        public string VerificationCode { get; set; }
    }
}
