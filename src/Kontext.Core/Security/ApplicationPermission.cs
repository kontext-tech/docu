namespace Kontext.Security
{
    /// <summary>
    /// Application permission definition
    /// </summary>
    public sealed class ApplicationPermission
    {
        public ApplicationPermission()
        { }

        public ApplicationPermission(string name, string value, string groupName, string description = null, bool isAdministrative = false)
        {
            Name = name;
            Value = value;
            GroupName = groupName;
            Description = description;
            IsAdministrative = isAdministrative;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public bool IsAdministrative { get; set; }

        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Support implicit conversion from permission to string
        /// </summary>
        /// <param name="permission"></param>
        public static implicit operator string(ApplicationPermission permission)
        {
            return permission.Value;
        }
    }
}
