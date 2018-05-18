using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPascal.Utils
{
    using SymbolTables = System.Collections.Generic.Dictionary<Tuple<string, int, int>, SymbolTable>;
    using Key = System.Tuple<string, int, int>;

    public class SymbolTableManager
    {
        private SymbolTables tables;
        public SymbolTable currentTable;
        public SymbolTableManager(SymbolTables tables)
        {
            this.tables = tables;
            this.currentTable = null;
        }

        public void enterScope(string scopeName, int scopeLevel, int id)
        {
            Key key = new Key(scopeName, scopeLevel, id);
            this.currentTable = tables[key];
        }

        public void exitScope()
        {
            this.currentTable = this.currentTable.baseScope;
        }
    }
}
