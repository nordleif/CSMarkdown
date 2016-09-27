using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace CSMarkdown.DataTable
{
    class Rows : ReadOnlyCollection<Row>
    {
        public Rows(IList<Row> list) : base(list)
        {

        }

        new public Cell[] this[int index]
        {
            get
            {
                
                return null;
            }
        }
    }
}
