using Mono.TextEditor;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using System;
using System.IO;

namespace DateInserter
{
    public class LinesCountWriter
    {
        private TextEditorData textEditorData;
        private Document linesCountDocument;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateInserter.LinesCountWriter"/> class.
        /// </summary>
        /// <param name="linesCountDocumentName">Lines count document name.</param>
        public LinesCountWriter(string linesCountDocumentName)
        {
            linesCountDocument = IdeApp.Workbench.GetDocument(linesCountDocumentName);
            if (linesCountDocument == null)
                linesCountDocument = IdeApp.Workbench.NewDocument(linesCountDocumentName, "text/plain", "");
            textEditorData = linesCountDocument.GetContent<ITextEditorDataProvider>().GetTextEditorData();
        }

        /// <summary>
        /// Writes the info of selected item.
        /// </summary>
        /// <param name="selectedItem">Selected item.</param>
        public void WriteInfoOfSelectedItem(object selectedItem)
        {
            textEditorData.Document.Text = "";
            Solution selectedSolution = selectedItem as Solution;
            if (selectedSolution != null)
                ShowSolution(selectedSolution);
            
            Project selectedProject = selectedItem as Project;
            if (selectedProject != null)
                ShowProject(selectedProject);
            
            ProjectFile selectedFile = selectedItem as ProjectFile;
            if (selectedFile != null)
                ShowProjectFile(selectedFile);

            linesCountDocument.Select();
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

        private void Write(string info)
        {
            textEditorData.InsertAtCaret(info + "\n");
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

    }
}

