using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using Mono.TextEditor;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace DateInserter
{
    public class InsertDateHandler : CommandHandler
    {
        private const string lINES_COUNT_DOCUMENT_NAME = "Lines Count Statistics";
        private TextEditorData textEditorData;

        protected override void Update(CommandInfo info)
        {
            info.Enabled = GetSelectedItem() != null;
        }

        protected override void Run()
        {
            PrepareLinesCountDocument();
            object selectedItem = GetSelectedItem();
            if (selectedItem == null)
                WriteEvathing();
            else
                WriteInfoOfSelectedItem(selectedItem);
        }

        void PrepareLinesCountDocument()
        {
            Document linesCountDocument = IdeApp.Workbench.GetDocument(lINES_COUNT_DOCUMENT_NAME);
            if (linesCountDocument == null)
                linesCountDocument = IdeApp.Workbench.NewDocument(lINES_COUNT_DOCUMENT_NAME, "text/plain", "");
            textEditorData = linesCountDocument.GetContent<ITextEditorDataProvider>().GetTextEditorData();
        }

        static object GetSelectedItem()
        {
            return IdeApp.ProjectOperations.CurrentSelectedItem;
        }

        private void WriteEvathing()
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
                    Write(si.Name);
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

        private void ShowProjectFile(ProjectFile projectFile)
        {
            if (projectFile != null)
            {
                if (projectFile.Subtype == Subtype.Code && projectFile.FilePath.ToString().Trim().EndsWith(".cs"))
                    WriteFileInfo(projectFile.FilePath);
            }
        }

        private void WriteFileInfo(FilePath filePath)
        {
            string[] lines = File.ReadAllLines(filePath.ToString());
            string fittingPath = filePath.ToString().Length > 80 ? filePath.ToString().Substring(0, 79) : filePath.ToString();
            string info = String.Format("{0, -80} {1, 3}", fittingPath, lines.Length);
            Write("   " + info);
        }

        private void WriteInfoOfSelectedItem(object selectedItem)
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
}

