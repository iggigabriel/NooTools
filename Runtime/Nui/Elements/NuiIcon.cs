namespace Noo.Nui
{
    public abstract partial class NuiIcon : NuiText
    {
        public NuiIcon() : base()
        {
            AddToClassList("nui-icon");
            parseEscapeSequences = true;
        }
    }
}
