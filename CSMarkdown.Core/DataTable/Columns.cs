using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace CSMarkdown.DataTable
{
    class Columns : ReadOnlyCollection<Column>
    {

        public Columns(IList<Column> list) : base(list)
        {
            
        }
        new public Cell[] this[int index]
        {
            get {

                return null;

                 }
            
        }
    }
}
