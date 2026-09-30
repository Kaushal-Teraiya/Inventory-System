namespace InventorySystem.Domain
{
    public readonly struct ItemId
    {
        public string Value {get;}

        public ItemId(string value)
        {
            Value = value;
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(Value);

        public override string ToString()
        {
            return Value;
        }
    }   
}