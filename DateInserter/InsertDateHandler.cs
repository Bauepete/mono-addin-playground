using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using Mono.TextEditor;
using System;
using System.Collections.ObjectModel;

namespace DateInserter
{
    public class InsertDateHandler : CommandHandler
    {
        private TextEditorData textEditorData;

        protected override void Update(CommandInfo info)
        {
            Document doc = IdeApp.Workbench.ActiveDocument;  
            info.Enabled = doc != null && doc.GetContent<ITextEditorDataProvider>() != null;
        }

        protected override void Run()
        {
            Document doc = IdeApp.Workbench.ActiveDocument;
            textEditorData = doc.GetContent<ITextEditorDataProvider>().GetTextEditorData();  
            Write("ApplicationRootPath: " + FileService.ApplicationRootPath);
            ReadOnlyCollection<Solution> solutions = IdeApp.Workspace.GetAllSolutions();


            foreach (Solution s in solutions)
            {
                Write("Solution's file name: " + s.FileName);
                Write("Solution's root folder: " + s.RootFolder.Name);

                ShowSolution(s.RootFolder);
            }
        }

        private void Write(string info)
        {
            textEditorData.InsertAtCaret(info + "\n");
        }

        private void ShowSolution(SolutionFolder solution)
        {
            foreach (SolutionItem si in solution.Items)
            {
                Project project = si as Project;
                if (project != null)
                    ShowProject(project);
                else
                {
                    Write(si.Name);
                }
            }
        }

        private void ShowProject(Project project)
        {
            Write("In project " + project.Name);
            foreach (ProjectItem projectItem in project.Items)
            {
                ProjectFile projectFile = projectItem as ProjectFile;
                if (projectFile != null)
                {
                    if (projectFile.Subtype == Subtype.Code && projectFile.FilePath.ToString().Trim().EndsWith(".cs"))
                        Write("   " + projectFile.FilePath);
                }
            }
        }
    }
}

