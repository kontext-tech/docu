namespace Kontext.Data.Models.ViewModels
{
    /// <summary>
    /// View models for information about one enum item
    /// </summary>
    public sealed class EnumItemViewModel
    {
        public int EnumValue { get; set; }
        public string EnumName { get; set; }
        public bool IsSelected { get; set; }
    }
}
