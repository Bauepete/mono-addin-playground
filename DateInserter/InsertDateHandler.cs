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
        private const string lINES_COUNT_DOCUMENT_NAME = "Lines Count Statistics";

        private TextEditorData textEditorData;

        protected override void Update(CommandInfo info)
        {
            info.Enabled = true; //openDocuments != null && openDocuments.GetContent<ITextEditorDataProvider>() != null;
        }

        protected override void Run()
        {

            Document linesCountDocument = IdeApp.Workbench.GetDocument(lINES_COUNT_DOCUMENT_NAME);
            if (linesCountDocument == null)
                linesCountDocument = IdeApp.Workbench.NewDocument(lINES_COUNT_DOCUMENT_NAME, "text/plain", "");
            
//            Document linesCountDocument = IdeApp.Workbench.ActiveDocument;
            textEditorData = linesCountDocument.GetContent<ITextEditorDataProvider>().GetTextEditorData();

            object selectedItem = IdeApp.ProjectOperations.CurrentSelectedItem;
            if (selectedItem == null)
                WriteEvathing();
            else
            {
                Solution selectedSolution = selectedItem as Solution;
                if (selectedSolution != null)
                    ShowSolution(selectedSolution);
                
                Project selectedProject = selectedItem as Project;
                if (selectedProject != null)
                    ShowProject(selectedProject);

                ProjectFile selectedFile = selectedItem as ProjectFile;
                if (selectedFile != null)
                    ShowProjectFile(selectedFile);
            }

        }

        void WriteEvathing()
        {
            Write("ApplicationRootPath: " + FileService.ApplicationRootPath);
            ReadOnlyCollection<Solution> solutions = IdeApp.Workspace.GetAllSolutions();
            foreach (Solution s in solutions)
            {
                Write("Solution's file name: " + s.FileName);
                Write("Solution's root folder: " + s.RootFolder.Name);
                ShowSolution(s);
            }
        }

        private void Write(string info)
        {
            textEditorData.InsertAtCaret(info + "\n");
        }

        private void ShowSolution(Solution solution)
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
                ShowProjectFile(projectFile);
            }
        }

        void ShowProjectFile(ProjectFile projectFile)
        {
            if (projectFile != null)
            {
                if (projectFile.Subtype == Subtype.Code && projectFile.FilePath.ToString().Trim().EndsWith(".cs"))
                    Write("   " + projectFile.FilePath);
            }
        }
    }
}

