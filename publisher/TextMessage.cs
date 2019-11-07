namespace publisher
{
    public class TextMessage
    {
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}