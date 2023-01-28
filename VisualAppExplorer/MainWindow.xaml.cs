using System;
using System.Collections.Generic;
using System.Windows;
using AppExplorer;
using AppExplorer.Packages;
using Microsoft.Win32;

namespace VisualAppExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> ExcludedNamespaces = new List<string>();

        public MainWindow()
        {
            DiagramMakerTypes = new()
            {
                $"{nameof(D2Package)}",
                nameof(MDPackage)
            };
            InitializeComponent();
        }

        public List<string> DiagramMakerTypes { get; set; }

        public string Error { get; set; } = "";

        private void btnOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            string selected = (string)LayoutManager.SelectedItem;
            if (selected == null)
            {
                ErrorLabel.Text = "Select a layout manager";
                return;
            }

            ErrorLabel.Text = "";

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "DLL files (*.dll)|*.dll|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;

                TypeDefiner PackageMaker = new TypeDefiner();

                IDiagram diagram;
                if (selected == nameof(D2Package))
                    diagram = new D2Package();
                else if (selected == nameof(MDPackage))
                    diagram = new MDPackage();
                else
                {
                    ErrorLabel.Text = "Select a layout manager";
                    return;
                }

                string output = PackageMaker.ReadDllTypes(path, diagram);
                if (selected == nameof(D2Package))
                    SVGViewer.Source = new Uri(output, UriKind.Absolute);
            }
        }
    }
}