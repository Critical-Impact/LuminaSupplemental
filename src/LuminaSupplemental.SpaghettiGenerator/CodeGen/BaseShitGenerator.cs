using System.Text;

namespace LuminaSupplemental.SpaghettiGenerator.CodeGen
{
    public class BaseShitGenerator
    {
        protected BaseShitGenerator( string typeName, string fieldName, uint columnId )
        {
            TypeName = typeName;
            FieldName = fieldName;
            ColumnId = columnId;
        }

        protected BaseShitGenerator()
        {
            
        }

        protected string TypeName { get; set; }
        protected string FieldName { get; set; }
        protected uint ColumnId { get; set; }
        
        public virtual void WriteFields( StringBuilder sb )
        {
        }

        public virtual void WriteReaders( StringBuilder sb )
        {
        }

        public virtual void WriteStructs( StringBuilder sb )
        {
        }
    }
}