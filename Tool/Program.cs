﻿
GenerateAst.Tun(args);

public class GenerateAst
{
    public static void Tun(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: GenerateAst <output directory>");
            System.Environment.Exit(64);
        }
        String outputDir = args[0];
        
        DefineAst(outputDir, "Expr", new List<string>{
            "Assign   : Token name, Expr value",
            "Binary   : Expr left, Token op, Expr right",
            "Grouping : Expr expr",
            "Literal  : object value",
            "Unary    : Token op, Expr right",
            "Variable : Token name"
        });

        DefineAst(outputDir, "Stmt", new List<string>{
            "Expression  : Expr expr",
            "Print       : Expr expr",
            "Var         : Token name, Expr? initializer"
        });
    }

    private static string GetCsConventionalFieldName(string fieldName)
    {
        return fieldName[0].ToString().ToUpper() + fieldName.Substring(1);
    }

    private static void DefineAst(string outputDir, String baseName, List<string> types)
    {
        string path = Path.Combine(outputDir, $"{baseName}.cs");
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("namespace MDA;");
            writer.WriteLine();
            writer.WriteLine("public abstract class " + baseName + " {");
            
            DefineVisitor(writer, baseName, types);
            
            // The AST classes
            foreach (var type in types)
            {
                string className = type.Split(':')[0].Trim();
                string fields = type.Split(':')[1].Trim();
                DefineType(writer, baseName, className, fields);
            }
            
            // The base accept() method
            writer.WriteLine();
            writer.WriteLine("  public abstract T Accept<T>(IVisitor<T> visitor);");

            writer.WriteLine("}");
        }
    }

    private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
    {
        writer.WriteLine("  public class " + className + " : " + baseName +  " {");
        
        // Constructor
        writer.WriteLine("    public " + className + "(" + fieldList + ") {");
        
        // Store parameters in fields.
        string[] fields = fieldList.Split(',');
        
        foreach (var field in fields)
        {
            string name = field.Trim().Split(" ")[1];
            writer.WriteLine("      this." + GetCsConventionalFieldName(name) + " = " + name + ";");
        }
        
        writer.WriteLine("    }");
        
        // Visitor pattern.
        writer.WriteLine();
        // writer.WriteLine("  @oOverride");
        writer.WriteLine("     public override T Accept<T>(IVisitor<T> visitor) {");
        writer.WriteLine("      return visitor.Visit" + className + baseName + "(this);");
        writer.WriteLine("    }");
        
        // Fields.
        writer.WriteLine();
        foreach (var field in fields)
        {
            string type = field.Trim().Split(" ")[0].Trim();
            string name = field.Trim().Split(" ")[1].Trim();
            writer.WriteLine("    public " + type + " " + GetCsConventionalFieldName(name) + " { get; set; }");
        }
        
        writer.WriteLine("  }");
        writer.WriteLine();
    }

    private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
    {
        writer.WriteLine("  public interface IVisitor <T> {");

        foreach (string type in types)
        {
            string typeName = type.Split(":")[0].Trim();
            writer.WriteLine("    T Visit" + typeName + baseName + "(" + typeName + " " + baseName.ToLower() + ");");
        }
        
        writer.WriteLine("  }");
        writer.WriteLine();
    }
}