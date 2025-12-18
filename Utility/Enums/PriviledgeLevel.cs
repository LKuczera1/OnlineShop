namespace Utility.Enums
{
    public enum PriviledgeLevel
    {
        NotAssigned = 0,
        Admin = 1,
        SalesDepartmentWorker = 2,
        Customer = 3,
    }


    public abstract class RolesStr
    {
        public const string NotAssigned = "NotAssigned";
        public const string Admin = "Admin";
        public const string SalesDepartmentWorker = "SalesDepartmentWorker";
        public const string Customer = "Customer";

        //Combinations
        public const string Admin_SalesDepartmentWorker = Admin + "," + SalesDepartmentWorker;
        public const string Admin_Customer = Admin + "," + Customer;
        public const string Admin_SalesDepartmentWorker_Customer = Admin + "," + SalesDepartmentWorker + "," + Customer;


        //abandoned
        public static string ToRoleString(params string[] values)
        {
            string str = string.Empty;

            foreach (var v in values)
            {
                str += v;
                str += ",";
            }

            return str;
        }

        public static PriviledgeLevel RoleToEnum(string role)
        {
            switch (role)
            {
                case RolesStr.Admin: return PriviledgeLevel.Admin;
                case RolesStr.Customer: return PriviledgeLevel.Customer;
                case RolesStr.SalesDepartmentWorker: return PriviledgeLevel.SalesDepartmentWorker;
                case RolesStr.NotAssigned: return PriviledgeLevel.NotAssigned;
                default: return PriviledgeLevel.NotAssigned;
            }
        }
    }
}


