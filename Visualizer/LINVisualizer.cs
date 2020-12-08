using System;
using System.Windows.Forms;
using CommandLine;
using LINVAST.Exceptions;
using LINVAST.Imperative;
using LINVAST.Nodes;
using Serilog;

namespace LINVisualizer
{
    public sealed class LINVisualizer
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
            => Parser.Default.ParseArguments<Options>(args).WithParsed(Visualize);


        private static void Visualize(Options o)
        {
            SetupLogger(o.Verbose);

            if (string.IsNullOrWhiteSpace(o.Source)) {
                Log.Fatal("Missing source path");
                Environment.Exit(1);
            }

            if (!TryBuildFromFile(o.Source, out ASTNode? ast) || ast is null)
                Environment.Exit(1);

            Log.Information("AST created");

            Log.Information("Showing tree...");
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new VisualizeForm(ast));

            Environment.Exit(0);
        }

        private static bool TryBuildFromFile(string path, out ASTNode? ast)
        {
            Log.Information("Creating AST for file: {Path}", path);

            ast = null;
            try {
                ast = new ImperativeASTFactory().BuildFromFile(path);
                return true;
            } catch (SyntaxErrorException e) {
                Log.Fatal(e, "[{Path}] Syntax error: {Details}", path, e.Message ?? "unknown");
            } catch (NotImplementedException e) {
                Log.Fatal(e, "[{Path}] Not supported: {Details}", path, e.Message ?? "unknown");
            } catch (UnsupportedLanguageException e) {
                Log.Fatal(e, "[{Path}] Not supported language", path);
            } catch (Exception e) {
                Log.Fatal(e, "[{Path}] Exception occured", path);
            }

            return false;
        }

        private static void SetupLogger(bool verbose)
        {
            LoggerConfiguration lcfg = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "\r[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .Enrich.FromLogContext()
                ;

            if (verbose)
                lcfg.MinimumLevel.Verbose();
            else
                lcfg.MinimumLevel.Information();

            Log.Logger = lcfg.CreateLogger();
        }
    }
}
