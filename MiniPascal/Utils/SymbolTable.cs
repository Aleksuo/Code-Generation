using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPascal.Utils
{
    public enum BuiltType
    {
        INTEGER, STRING, BOOLEAN, ERROR, REAL, NONE
    }

    public enum Category
    {
        FUNCTION, PROCEDURE, VARIABLE, PARAMETER, REFERENCE
    }
    public class Symbol {
        public string var;
        public BuiltType type;
        public Category category;

        public Symbol(string var, BuiltType type, Category category)
        {
            this.var = var;
            this.type = type;
            this.category = category;
        }

        override
        public string ToString()
        {
            return "(VARIABLE: " + var + ",TYPE: " + type.ToString()+", CATEGORY: "+category+")";
        }
    }

    public class ProcedureSymbol : Symbol
    {
        public List<Symbol> parameters;
        public ProcedureSymbol(string var, BuiltType type, Category category, List<Symbol> parameters) : base(var, type, category)
        {
            this.parameters = parameters;
        }
    }

    public class FunctionSymbol : Symbol
    {
        public List<Symbol> parameters;
        public FunctionSymbol(string var, BuiltType type , Category category, List<Symbol> parameters) : base(var, type, category)
        {
            this.parameters = parameters;
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

        public Symbol lookupOnlyThisScope(string name)
        {
            if (symbols.ContainsKey(name))
            {
                return this.symbols[name];
            }
            return null;
        }

        public Symbol lookup(string name)
        {
            if (symbols.ContainsKey(name))
            {
                return this.symbols[name];
            }
            else if (this == this.baseScope && !symbols.ContainsKey(name))
            {
                return null;
            }
            return this.baseScope.lookup(name);
        }

        public Symbol lookupInEnclosingProcedureOrFunction(string symbolName)
        {
            Symbol funcOrProc = lookup(this.name);
            if(funcOrProc == null)
            {
                return null;
            }
            if(funcOrProc.category == Category.PROCEDURE)
            {
                ProcedureSymbol cast = (ProcedureSymbol)funcOrProc;
                foreach (Symbol s in cast.parameters)
                {
                    if(s.var == symbolName)
                    {
                        return s;
                    }
                }
            }
            else
            {
                FunctionSymbol cast = (FunctionSymbol)funcOrProc;
                foreach (Symbol s in cast.parameters)
                {
                    if (s.var == symbolName)
                    {
                        return s;
                    }
                }
            }
           
            return null;
        }

        public override bool Equals(object obj)
        {
            SymbolTable other = (SymbolTable)obj;
            return ((this.name == other.name) && (this.scopeLevel == other.scopeLevel));
        }
    }
}
