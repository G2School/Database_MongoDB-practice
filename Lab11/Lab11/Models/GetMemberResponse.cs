namespace Lab11.Models
{
    public class GetMemberResponse
    {
        public bool ok { get; set; }

        public string errMsg { get; set; }

        public MemberInfo data { get; set; }

        public GetMemberResponse()
        {
            this.ok = true;
            this.errMsg = "";
            this.data = new MemberInfo();
        }
    }
}