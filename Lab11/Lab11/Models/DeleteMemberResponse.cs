﻿namespace Lab11.Models
{
    public class DeleteMemberResponse
    {

        public bool ok { get; set; }

        public string errMsg { get; set; }

        public DeleteMemberResponse()
        {
            this.ok = true;
            this.errMsg = "";
        }
    }
}