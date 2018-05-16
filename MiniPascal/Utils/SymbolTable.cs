using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPascal.Utils
{
    public enum BuiltType
    {
        INTEGER, STRING, BOOLEAN, ERROR, REAL
    }
    public class Symbol {
        public string var;
        public BuiltType type;
        public Symbol(string var, BuiltType type)
        {
            this.var = var;
            this.type = type;
        }

        override
        public string ToString()
        {
            return "(VARIABLE: " + var + ",TYPE: " + type.ToString()+")";
        }
    }

    public class SymbolTable
    {
        private Dictionary<string, Symbol> symbols;
        private Dictionary<string, BuiltType> types;
        public string name;
        public int scopeLevel;
        public SymbolTable baseScope;


        public SymbolTable(string name, int scopeLevel, SymbolTable baseScope)
        {
            this.symbols = new Dictionary<string, Symbol>();
            this.types = new Dictionary<string, BuiltType>();
            this.initTypes();
            this.name = name;
            this.scopeLevel = scopeLevel;
            this.baseScope = baseScope;
        }

        public void initTypes()
        {
            this.types["integer"] = BuiltType.INTEGER;
            this.types["Real"] = BuiltType.REAL;
            this.types["Boolean"] = BuiltType.BOOLEAN;
            this.types["string"] = BuiltType.STRING;
        }

        public void define(Symbol symbol)
        {
            //Console.WriteLine(symbol);
            this.symbols[symbol.var] = symbol;
        }

        public BuiltType lookType(string type)
        {
            return this.types[type];
        }

        public Symbol lookup(string name)
        {
            if (symbols.ContainsKey(name))
            {
                return this.symbols[name];
            }
            return null;           
        }
    }
}
