﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Auth.Dtos.Role
{
    public class UpdateRoleDto
    {
        private string _name = String.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Không được bỏ trống")]
        required public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Không được bỏ trống")]
        public string Name { get => _name; set => _name = value.Trim(); }

        public List<string> PermissionKey { get; set; } = new List<string>();
    }
}
