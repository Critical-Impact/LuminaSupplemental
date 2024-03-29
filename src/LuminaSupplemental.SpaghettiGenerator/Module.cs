namespace LuminaSupplemental.SpaghettiGenerator
{
    public abstract class Module
    {
        protected DatabaseBuilder _builder;

        protected Module ()
        {
            _builder = DatabaseBuilder.Instance;
        }

        public abstract string Name { get; }

        public abstract void Start();
    }
}
