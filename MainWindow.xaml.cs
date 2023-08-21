using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Collections.Concurrent;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Media.Effects;

namespace DiskManager
{

    public partial class MainWindow : Window
    {
        public List<ProgressBarItem> ProgressBarItems { get; set; }
        private List<string> foundFiles = new List<string>();

        private ListBox displayFiles; // Declare displayFiles ListBox here

        private Slider numberSlider; // Declare numberSlider as a private member of the MainWindow class

        private TextBlock selectedNumberTextBlock; // Declare selectedNumberTextBlock as a private member of the MainWindow class
        private TextBlock fileCountTextBlock; // count Large Files
        private TextBlock ResultTextBlock;
        private TextBlock folderPathTextBox;


        private ComboBox fileTypeComboBox;// Declare the ComboBox as a member variable of the class
        private ComboBox duplicateFileTypeComboBox;



        private Button browseFolderButton;
        private Button browseFileButton;
        private Button deleteButton;
        private Button findLargeSizedFilesButton; // Large File size button

        private double minSize;
        private long count = 0;
        private string fileFormat = "";

        string[] allFiles;

        Dictionary<string, double> categorySizes = new Dictionary<string, double>(); // DICTIONARY TO KEEP VALUES IN PIE CHART
        private Dictionary<string, List<string>> duplicateFilesMap = new Dictionary<string, List<string>>();



        public MainWindow() // intialize all
        {

            InitializeComponent();
            ProgressBarItems = new List<ProgressBarItem>();
            createContent();
            DataContext = this;
        }

        public class ProgressBarItem
        {
            public string Name { get; set; }
            public long Progress { get; set; }
            public long MaxValue { get; set; }
            public string Size { get; set; }
        }


        private Style CreateRoundButtonStyle()
        {
            Style roundButtonStyle = new Style(typeof(Button));

            // Set the template for the button
            roundButtonStyle.Setters.Add(new Setter(Control.TemplateProperty, CreateButtonTemplate()));

            // Set other properties for the button
            roundButtonStyle.Setters.Add(new Setter(Button.HeightProperty, 40.0)); // Increase height by 100
            roundButtonStyle.Setters.Add(new Setter(Button.WidthProperty, 400.0));
            roundButtonStyle.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(10)));
            roundButtonStyle.Setters.Add(new Setter(Button.MarginProperty, new Thickness(5)));

            return roundButtonStyle;
        }







        private ControlTemplate CreateButtonTemplate()
        {
            ControlTemplate buttonTemplate = new ControlTemplate(typeof(Button));

            // Create the border to represent the button
            FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.Name = "roundedRectangle";

            // Set the button background color to #FEEADA
            borderFactory.SetValue(Border.BackgroundProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEEADA")));

            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(6.0));

            // Create the content presenter to display the button content
            FrameworkElementFactory contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            borderFactory.AppendChild(contentPresenterFactory);

            // Set the visual tree of the control template
            buttonTemplate.VisualTree = borderFactory;

            // Define triggers for different button states
            Trigger mouseOverTrigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };

            // Set the hover color for the button (light gray)
            mouseOverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Colors.LightGray)));

            buttonTemplate.Triggers.Add(mouseOverTrigger);

            return buttonTemplate;
        }

        void CreateAbout()
        {

            // Create the StackPanel to hold the content
            StackPanel contentStackPanel = new StackPanel
            {
                Margin = new Thickness(20),
                VerticalAlignment = VerticalAlignment.Center
            };


            // Create the TextBlock to display the instruction
            TextBlock Motivation = new TextBlock
            {
                Text = "MOTIVATION",
                FontSize = 22,
                FontWeight = FontWeights.ExtraBold,
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center

            };

            TextBlock Motivation_Text = new TextBlock
            {
                Text = "This application was developed during a Hackathon organised by Spark under the event Spark August Hackathon,23 ," +
           " showcasing rapid development skills. " +
           "dedication to creating innovative solutions within a tight timeframe. ",
                FontSize = 16,
                FontWeight = FontWeights.DemiBold,
                Margin = new Thickness(5),
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
            };

            TextBlock TeamMembers = new TextBlock
            {
                Text = "\"Brilliantly crafted by innovative minds\"",
                FontSize = 22,
                FontWeight = FontWeights.ExtraBold,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(5, 5, 5, 20),
                HorizontalAlignment = HorizontalAlignment.Center

            };

            contentStackPanel.Children.Add(TeamMembers);
            List<Profile> profiles = new List<Profile>
            {
                new Profile { Name = "Abhishek Mallick", Email = "mallickabhishek97@gmail.com@gmail.com", Position = "Pyhton FullStack Developer" }
            };

            // Create UI elements dynamically for each profile
            foreach (var profile in profiles)
            {
                StackPanel profileInfoPanel = new StackPanel();

                TextBlock nameTextBlock = new TextBlock
                {
                    Text = profile.Name,
                    FontSize = 16,
                    FontWeight = FontWeights.DemiBold,
                    TextWrapping = TextWrapping.Wrap
                };
                profileInfoPanel.Children.Add(nameTextBlock);

                TextBlock emailTextBlock = new TextBlock
                {
                    Text = profile.Email,
                    FontSize = 10,
                    TextWrapping = TextWrapping.Wrap
                };
                profileInfoPanel.Children.Add(emailTextBlock);

                TextBlock positionTextBlock = new TextBlock
                {
                    Text = profile.Position,
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap
                };
                profileInfoPanel.Children.Add(positionTextBlock);

                // Add the profile info panel to the main StackPanel (UI Element)
                Border profileBorder = new Border
                {
                    BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#33334C")),
                    BorderThickness = new Thickness(0.2),
                    Margin = new Thickness(0, 0, 0, 10), // Add some margin between profiles
                    Padding = new Thickness(5), // Add some padding within the border
                    Child = profileInfoPanel
                };

                // Apply box shadow to the Border dynamically
                DropShadowEffect dropShadow = new DropShadowEffect
                {
                    BlurRadius = 5,
                    ShadowDepth = 2,
                    Color = Colors.Black,
                    Opacity = 0.3
                };

                profileBorder.Effect = dropShadow;
                contentStackPanel.Children.Add(profileBorder);
            }





            contentStackPanel.Children.Add(Motivation);
            contentStackPanel.Children.Add(Motivation_Text);
            // Clear the previous content from the MAIN_AREA StackPanel
            MAIN_AREA.Children.Clear();

            // Add the content StackPanel to the MAIN_AREA StackPanel
            MAIN_AREA.Children.Add(contentStackPanel);


        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            MAIN_AREA.Children.Clear();
            CreateAbout();



        }


        // ------------------------------------------------------------- DELETE ------------------------------------------------------------------------------
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            MAIN_AREA.Children.Clear();

            StackPanel deleteFolder = new StackPanel();

            folderPathTextBox = new TextBlock
            {

                Text = "Path : ",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
            };

            Border border = new Border
            {
                BorderThickness = new Thickness(3), // Thicker border (e.g., 3 pixels)
                BorderBrush = Brushes.Black,
                Margin = new Thickness(5), // Margin of 10 units on all sides
                Padding = new Thickness(10), // left top right bottom

                Child = folderPathTextBox // Set the StackPanel as the child of the Border
            };



            browseFolderButton = new Button
            {
                Margin = new Thickness(10, 50, 10, 10),
                Content = "Choose folder"
            };
            browseFolderButton.Click += BrowseButton_Click;
            browseFolderButton.Style = CreateRoundButtonStyle();
            deleteFolder.Children.Add(new TextBlock
            {
                Text = "     ",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            }
            );

            browseFileButton = new Button
            {
                Margin = new Thickness(10, 50, 10, 10),
                Content = "Choose Specific File"
            };
            browseFileButton.Click += BrowseFileButton_Click;
            browseFileButton.Style = CreateRoundButtonStyle();
            deleteFolder.Children.Add(new TextBlock
            {
                Text = "     ",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            }
            );

            deleteButton = new Button
            {
                Margin = new Thickness(10, 50, 10, 10),
                VerticalAlignment = VerticalAlignment.Bottom,
                Content = "Delete"
            };
            deleteFolder.Children.Add(new TextBlock
            {
                Text = "     ",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            }
            );
            deleteButton.Click += DeleteFolderButton_Click;
            deleteButton.Style = CreateRoundButtonStyle();


            deleteFolder.Children.Add(border);
            deleteFolder.Children.Add(browseFolderButton);
            deleteFolder.Children.Add(browseFileButton);
            deleteFolder.Children.Add(deleteButton);
            MAIN_AREA.Children.Add(deleteFolder);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folderPath = folderBrowserDialog.SelectedPath; ;
                folderPathTextBox.Text = "Path : " + folderPath;

            }
        }

        private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a File",
                Filter = "All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                folderPathTextBox.Text = "Path : " + selectedFilePath;
                MessageBox.Show($"Selected file: {selectedFilePath}");
            }
        }
        private void DeleteFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = folderPathTextBox.Text.Substring(7); ;

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                System.Windows.MessageBox.Show("Please enter a valid folder path or select a file.");
                return;
            }

            try
            {
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                    System.Windows.MessageBox.Show("Folder deleted successfully.");
                    folderPathTextBox.Text = "Path : ";
                }
                else if (System.IO.File.Exists(folderPath))
                {
                    System.IO.File.Delete(folderPath);
                    System.Windows.MessageBox.Show("File deleted successfully.");
                    folderPathTextBox.Text = "Path : ";
                }
                else
                {
                    System.Windows.MessageBox.Show("The folder or file does not exist.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        // -------------------------------------------------------- BUTTON 2 (SEARCH) -------------------------------------------------------------



        void CreateAndAddContent()
        {

            // Create the StackPanel to hold the content
            StackPanel contentStackPanel = new StackPanel
            {
                Margin = new Thickness(20),
                VerticalAlignment = VerticalAlignment.Center
            };




            StackPanel searchByFileTypePanel = new StackPanel
            {
                Margin = new Thickness(20),
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            StackPanel duplicateFilePanel = new StackPanel
            {
                Margin = new Thickness(20),
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            // Create the TextBlock to display the instruction
            TextBlock instructionTextBlock = new TextBlock
            {
                Text = "View Panel",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center

            };



            TextBlock largeFileHeading = new TextBlock
            {
                Text = "Search By File Size : ",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5),

            };


            TextBlock fileTypeHeading = new TextBlock
            {
                Text = "Search By File Type : ",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5),

            };

            TextBlock duplicateFileHeading = new TextBlock
            {
                Text = "Search By Duplicate File : ",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5),

            };

            // Create the Slider for number selection
            numberSlider = new Slider
            {
                Minimum = 0,
                Maximum = 1000000000,
                TickFrequency = 1,
                Margin = new Thickness(0, 5, 0, 0)
            };
            numberSlider.ValueChanged += NumberSlider_ValueChanged;

            // Create the TextBlock to display the selected number from the slider
            selectedNumberTextBlock = new TextBlock
            {
                Text = "Selected Size: 0",
                FontSize = 14,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            // Create the ListBox to display the paths of large-sized files
            displayFiles = new ListBox
            {
                Width = double.NaN, // Set the width to double.NaN for auto-width
                Height = 250,
                MaxHeight = 300
            };

            // Create the TextBlock to display the number of large files
            fileCountTextBlock = new TextBlock
            {
                Text = "File Count : 0",
                Margin = new Thickness(5),
                FontSize = 14,
            };





            // Create the Button to trigger finding large-sized files
            findLargeSizedFilesButton = new Button
            {
                Content = "Search",
                Width = 100,
                Margin = new Thickness(5, 5, 10, 5),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            findLargeSizedFilesButton.Click += FindLargeSizedFiles_Click;
            findLargeSizedFilesButton.Style = CreateRoundButtonStyle();
            // Create the Delete Button
            Button deleteButton = new Button
            {
                Content = "Delete",
                Padding = new Thickness(10),
                FontSize = 14,
                Margin = new Thickness(0, 10, 0, 0)



            };
            deleteButton.Click += DeleteButton_Click;
            deleteButton.Style = CreateRoundButtonStyle();


            // Drop-down menu with values: (.txt, .docs, .pdf, .png, .jpg, .jpeg)
            fileTypeComboBox = new ComboBox
            {
                Width = 100,
                Margin = new Thickness(5),
                Padding = new Thickness(10)

            };
            fileTypeComboBox.Items.Add(".txt");
            fileTypeComboBox.Items.Add(".docx");
            fileTypeComboBox.Items.Add(".pdf");
            fileTypeComboBox.Items.Add(".png");
            fileTypeComboBox.Items.Add(".jpg");
            fileTypeComboBox.Items.Add(".jpeg");

            // fileTypeComboBox.SelectedIndex = 0;
            fileTypeComboBox.SelectionChanged += FileTypeComboBox_SelectionChanged;

            Button searchButton = new Button
            {
                Content = "Search",
                Margin = new Thickness(5),
                Padding = new Thickness(10), // Add padding of 10 units
                Width = 100
            };
            searchButton.Click += SearchButton_Click;
            searchButton.Style = CreateRoundButtonStyle();


            duplicateFileTypeComboBox = new ComboBox
            {
                Width = 100,
                Margin = new Thickness(5, 5, 10, 5),
                Padding = new Thickness(10)
            };

            duplicateFileTypeComboBox.SelectedIndex = 0;
            duplicateFileTypeComboBox.Items.Add("All Files");
            duplicateFileTypeComboBox.Items.Add("Image");
            duplicateFileTypeComboBox.Items.Add("Video");
            duplicateFileTypeComboBox.Items.Add("Document");
            duplicateFileTypeComboBox.Items.Add("Audio");



            // Search button with drop-down menu

            Button duplicate_searchButton = new Button
            {
                Content = "Search",
                Margin = new Thickness(5),
                Padding = new Thickness(10), // Add padding of 10 units
                Width = 100
            };
            duplicate_searchButton.Click += SearchButton_Click2;
            duplicate_searchButton.Style = CreateRoundButtonStyle();


            searchByFileTypePanel.Children.Add(fileTypeComboBox);
            searchByFileTypePanel.Children.Add(searchButton);



            duplicateFilePanel.Children.Add(duplicateFileTypeComboBox);
            duplicateFilePanel.Children.Add(duplicate_searchButton);

            // Add the TextBox, ComboBox, and Search Button to the searchPanel

            StackPanel colouring = new StackPanel();
            contentStackPanel.Children.Add(instructionTextBlock);
            contentStackPanel.Children.Add(fileCountTextBlock);
            colouring.Children.Add(displayFiles); // Add the ListBox to the content StackPanel


            Border border = new Border
            {
                BorderThickness = new Thickness(3), // Thicker border (e.g., 3 pixels)
                BorderBrush = Brushes.Black,
                Margin = new Thickness(0.5), // Margin of 10 units on all sides
                // Padding = new Thickness(10), // left top right bottom
                Child = colouring // Set the StackPanel as the child of the Border
            };
            // Add all the elements to the content StackPanel
            contentStackPanel.Children.Add(border); // Add the Delete Button to the content StackPanel
            contentStackPanel.Children.Add(deleteButton); // Add the Delete Button to the content StackPanel
            contentStackPanel.Children.Add(
            new TextBlock
            {
                Text = "     ",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5),

            });
            contentStackPanel.Children.Add(largeFileHeading);
            contentStackPanel.Children.Add(numberSlider);
            contentStackPanel.Children.Add(selectedNumberTextBlock);
            contentStackPanel.Children.Add(findLargeSizedFilesButton);

            contentStackPanel.Children.Add(
            new TextBlock
            {
                Text = "     ",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5),

            });

            //    contentStackPanel.Children.Add(searchPanel); // Add the searchPanel to the content StackPanel
            contentStackPanel.Children.Add(fileTypeHeading);
            contentStackPanel.Children.Add(searchByFileTypePanel);
            contentStackPanel.Children.Add(
            new TextBlock
            {
                Text = "     ",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5),

            });
            contentStackPanel.Children.Add(duplicateFileHeading);


            contentStackPanel.Children.Add(duplicateFilePanel);



            displayFiles.SelectionMode = SelectionMode.Extended; // Multiple selection delete

            // Clear the previous content from the MAIN_AREA StackPanel
            MAIN_AREA.Children.Clear();

            // Add the content StackPanel to the MAIN_AREA StackPanel
            MAIN_AREA.Children.Add(contentStackPanel);


        }





        private void updateCount()
        {
            fileCountTextBlock.Text = $"File Count : {displayFiles.Items.Count}";
        }






        //  calculate duplicates from here 
        private void SearchButton_Click2(object sender, RoutedEventArgs e)
        {
            displayFiles.ItemsSource = null;
            string selectedFileType = duplicateFileTypeComboBox.SelectedItem as string;

            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string searchDirectory = folderBrowserDialog.SelectedPath;




                string searchPattern = GetSearchPattern(selectedFileType);
                if (searchPattern == null)
                {
                    System.Windows.MessageBox.Show("Invalid file type selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string[] patterns = searchPattern.Split("|");
                allFiles = null;

                string tmp = searchDirectory;
                foreach (var pattern in patterns)
                {
                    // MessageBox.Show(pattern.TrimEnd().TrimStart()); 
                    if (allFiles == null) allFiles = Directory.GetFiles(searchDirectory, "*" + pattern, SearchOption.AllDirectories);
                    else allFiles = allFiles.Concat(Directory.GetFiles(searchDirectory, "*" + pattern, SearchOption.AllDirectories)).ToArray();
                }
            }

            duplicateFilesMap.Clear();

            //  var progressDialog = new ProgressDialog();
            //  progressDialog.Show();

            // Perform the search process asynchronously using Tasks

            Task.Run(() =>
            {


                foreach (var file in allFiles)
                {
                    string fileHash = CalculateFileHash(file);

                    lock (duplicateFilesMap)
                    {
                        if (!duplicateFilesMap.ContainsKey(fileHash))
                        {
                            duplicateFilesMap.Add(fileHash, new List<string>());
                        }

                        duplicateFilesMap[fileHash].Add(file);
                    }
                }

                // Filter out non-duplicate files
                var duplicateFiles = duplicateFilesMap.Values.Where(d => d.Count > 1).ToList();

                // Update the ListView with the duplicate files
                foreach (var group in duplicateFiles)
                {
                    lock (foundFiles)
                    {
                        foreach (var file in group)
                        {
                            foundFiles.Add(file);
                        }
                    }
                }

                // Update the UI once the search process is complete
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    displayFiles.ItemsSource = foundFiles;
                    updateCount();

                    if (duplicateFiles.Count == 0)
                    {
                        displayFiles.ItemsSource = null;
                        updateCount();
                        System.Windows.MessageBox.Show("No duplicate files found.", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                });
            });


            // progressDialog.Close();
        }

        private static string GetSearchPattern(string fileType)
        {
            string tmp = "";
            if (fileType == "Image") tmp = ".jpg|.jpeg|.png|.gif|.bmp|.tiff|.tif|.webp|.svg";
            else if (fileType == "Video") tmp = ".mp4|.avi|.mkv|.mov|.wmv|.flv|.webm|.m4v|.mpeg|.mpg|.3gp|.3g2";
            else if (fileType == "Document") tmp = ".doc|.docx|.pdf|.rtf|.ppt|.pptx|.txt|.xls|.xlsx|.odt|.ods|.odp|.csv";
            else if (fileType == "Audio") tmp = ".mp3|.wav|.m4a|.flac|.ogg|.aac|.wma|.aiff|.alac|.amr|.opus";
            return tmp;
        }
        private string CalculateFileHash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLower();
                }
            }
        }


        //  duplicates file end 


        private void SearchButton_Click(object sender, RoutedEventArgs e)  // SPECIFIC FORMAT SEARCH 
        {
            displayFiles.ItemsSource = null;
            // Show a folder browser dialog to select the search directory
            foundFiles.Clear();
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string searchDirectory = folderBrowserDialog.SelectedPath;



                // Search for files with the specified format in the chosen directory
                foundFiles = Directory.GetFiles(searchDirectory, "*" + fileFormat, SearchOption.AllDirectories).ToList();

                // Display the found files in the list view
                displayFiles.ItemsSource = null; // Clear the ListBox's ItemsSource before updating
                displayFiles.ItemsSource = foundFiles;
                updateCount();
            }
            if (foundFiles.Count == 0)
            {
                System.Windows.MessageBox.Show("No file found.", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void FileTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fileTypeComboBox.SelectedItem != null)
            {
                // Get the selected item from the ComboBox
                fileFormat = fileTypeComboBox.SelectedItem as string;

                // Your code to handle the selection change goes here
                // For example, you can display a message with the selected file type:
                MessageBox.Show("Selected File Type: " + fileFormat);
            }
            else
            {
                // Handle the case when no item is selected
                MessageBox.Show("No File Type selected.");
            }
        }

        // SPECIFIC FORMAT SEARCH END


        // DELETE BUTTON 
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (displayFiles.SelectedItems.Count > 0)
            {
                try
                {
                    // Create a copy of the selected items since we'll be modifying the collection
                    //var selectedItems = displayFiles.SelectedItems.Cast<string>().ToList();

                    // Loop through the selected items and delete each file
                    foreach (var selectedItem in displayFiles.SelectedItems.OfType<string>().ToList())
                    {

                        System.IO.File.Delete(selectedItem);
                        foundFiles.Remove(selectedItem);

                    }

                    // Refresh the list view and update the count
                    displayFiles.Items.Refresh();
                    updateCount();
                }
                catch (Exception ex)
                {
                    // Handle any exception that occurred while deleting the files
                    System.Windows.MessageBox.Show($"Error deleting files: {ex.Message}");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please select files to delete.");
            }
        }


        // SLIDER
        private Tuple<double, string, double> give_slider_value(double total_value, double MAXVAL)
        {
            double total_size = total_value, actual_size = 0, difference = 0.3 * MAXVAL;
            string type = "Byte";
            if (total_size > 0.7 * MAXVAL) // GB
            {
                total_size = 1 + 9 * (total_size - 0.7 * MAXVAL) / difference;
                total_size = Math.Round(total_size, 2);
                actual_size = total_size * 1e9;
                type = "GB";
            }
            else if (total_size > 0.4 * MAXVAL) // MB
            {
                total_size = 1 + 1e3 * (total_size - 0.4 * MAXVAL) / difference;
                total_size = Math.Round(total_size, 1);
                actual_size = total_size * 1e6;
                type = "MB";
            }
            else if (total_size > 0.1 * MAXVAL) // KB
            {
                total_size = 1 + 1e3 * (total_size - 0.1 * MAXVAL) / difference;
                total_size = Math.Round(total_size, 1);
                actual_size = total_size * 1e3;
                type = "KB";
            }
            else
            {
                difference = 0.1 * MAXVAL;
                total_size = total_size * 1e3 / difference;
                actual_size = total_size;
                total_size = Math.Round(total_size);
            }
            return Tuple.Create(total_size, type, actual_size); // slider size , type , file size to search
        }

        private void NumberSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var res = give_slider_value(numberSlider.Value, numberSlider.Maximum);
            selectedNumberTextBlock.Text = $"Selected Number: {res.Item1} {res.Item2}";
        }

        //  ---------------------------        Find Function   ----------------------------------

        private void FindLargeSizedFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                count = 0;

                var res = give_slider_value(numberSlider.Value, numberSlider.Maximum);
                minSize = (long)res.Item3;

                var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

                var directoriesToProcess = new Queue<string>();

                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string rootDirectory = folderBrowserDialog.SelectedPath;
                    directoriesToProcess.Enqueue(rootDirectory);

                    while (directoriesToProcess.Count > 0)
                    {
                        string currentDir = directoriesToProcess.Dequeue();

                        CountLargeSizedFiles(currentDir);

                        // Update the ListBox to show the directories with large-sized files
                        Helper(currentDir);
                        updateCount();
                    }

                    if (count > 200)
                    {
                        System.Windows.MessageBox.Show("More than 200 directories contain large-sized files. Showing only the top 200 directories.", "Result Limit Reached", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private void Helper(string rootDirectory)
        {
            foundFiles.Clear();

            var directoriesToCheck = new Stack<string>();
            directoriesToCheck.Push(rootDirectory);

            //long limit = (long)1e9; // Set a limit for the number of directories to traverse
            int directoryCount = 0;

            int fileLimit = 200; // Set a limit for the number of files to display
            // long maxSize = 1024 * 1024 * 1024; // Set a limit for the file size (e.g., 1 GB)

            while (directoriesToCheck.Count > 0) // CHANGE 
            {
                string currentDir = directoriesToCheck.Pop();
                directoryCount++;

                foreach (string file in Directory.EnumerateFiles(currentDir))
                {
                    FileInfo fileInfo = new FileInfo(file);

                    if (fileInfo.Length >= minSize)
                    {
                        foundFiles.Add(file);
                    }
                }

                foreach (string subDirectory in Directory.EnumerateDirectories(currentDir))
                {
                    directoriesToCheck.Push(subDirectory);
                }
            }

            // Update the ItemsSource of the ListBox
            displayFiles.ItemsSource = null; // Clear the ListBox's ItemsSource before updating
            displayFiles.ItemsSource = foundFiles;
        }

        private void CountLargeSizedFiles(string directory)
        {
            try
            {
                count = 0;
                var directoriesToCheck = new Queue<string>();
                directoriesToCheck.Enqueue(directory);

                //long maxSize = 1024 * 1024 * 1024; // Set a limit for the file size (e.g., 10 GB)

                while (directoriesToCheck.Count > 0)
                {
                    string currentDir = directoriesToCheck.Dequeue();

                    foreach (string file in Directory.EnumerateFiles(currentDir))
                    {
                        FileInfo fileInfo = new FileInfo(file);

                        if (fileInfo.Length >= minSize)
                        {
                            count++;
                        }
                    }

                    foreach (string subDirectory in Directory.EnumerateDirectories(currentDir))
                    {
                        directoriesToCheck.Enqueue(subDirectory);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        //  ---------------------------        Find Function    End    ----------------------------------
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            CreateAndAddContent();
        }



        //  --------------------------------------------------------   BUTTON 1 (Disk Space)   ------------------------------------------------------
        // DISK UTILIZATION WINDOW -> BUTTON 1

        private void createContent()
        {
            MAIN_AREA.Children.Clear();
            ProgressBarItems.Clear(); // Clear the previous data (if any)

            StackPanel stackPanel = new StackPanel();
            StackPanel drivePanel = new StackPanel();

            // Create the Border and set its properties
            Border border = new Border
            {
                BorderThickness = new Thickness(3), // Thicker border (e.g., 3 pixels)
                BorderBrush = Brushes.Black,
                Margin = new Thickness(10), // Margin of 10 units on all sides
                Padding = new Thickness(0, 10, 0, 30), // left top right bottom

                Child = drivePanel // Set the StackPanel as the child of the Border
            };


            // Populate ProgressBarItems with DriveInfo data
            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                ProgressBarItems.Add(new ProgressBarItem
                {
                    Name = d.Name,
                    Progress = d.TotalFreeSpace,
                    MaxValue = d.TotalSize,
                    Size = Math.Round(d.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0), 2).ToString() + "GB Free of " + Math.Round(d.TotalSize / (1024.0 * 1024.0 * 1024.0), 2).ToString() + "GB"
                });
            }

            // Clear the previous ProgressBar controls from the StackPanel
            MAIN_AREA.Children.Clear();

            // Add a new ProgressBar for each item in ProgressBarItems
            for (int i = 0; i < ProgressBarItems.Count; i++)
            {
                var item = ProgressBarItems[i];

                ProgressBar progressBar = new ProgressBar
                {
                    Height = 30,
                    Minimum = 0,
                    Maximum = item.MaxValue,
                    Value = item.Progress,
                    VerticalAlignment = VerticalAlignment.Top, // Align the ProgressBar to the top of the StackPanel
                    HorizontalAlignment = HorizontalAlignment.Left, // Align the ProgressBar to the left side of the StackPanel
                    Margin = new Thickness(0.05 * 800, i == 0 ? 0.05 * 450 : 0.03 * 450, 0, 0) // Set left and top margins
                };

                // Set the Width of the ProgressBar relative to the StackPanel's width using DataBinding
                progressBar.SetBinding(ProgressBar.WidthProperty, new System.Windows.Data.Binding("ActualWidth") { Source = MAIN_AREA, Converter = new ProgressBarWidthConverter() });

                // Create a TextBlock to display the drive name and size
                TextBlock textBlock = new TextBlock
                {
                    Text = $"{item.Name} - {item.Size}",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0.05 * 800, 0, 0, 0), // Set left margin for all items
                    VerticalAlignment = VerticalAlignment.Top, // Align the TextBlock to the top of the StackPanel
                    HorizontalAlignment = HorizontalAlignment.Left // Align the TextBlock to the left side of the StackPanel
                };


                // Create a StackPanel to hold the ProgressBar and TextBlock


                // Add the ProgressBar and TextBlock to the StackPanel

                // Add the StackPanel to the MAIN_AREA StackPanel
                drivePanel.Children.Add(progressBar);
                drivePanel.Children.Add(textBlock);
            }

            stackPanel.Children.Add(border);
            stackPanel.Children.Add(new TextBlock
            {
                Text = "     ",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            }
            );

            Button diskBreakdownbutton = new Button
            {
                Content = "Disk Breakdown"
            };
            diskBreakdownbutton.Style = CreateRoundButtonStyle();
            diskBreakdownbutton.Click += ScanBtn_Click;
            diskBreakdownbutton.Click += ShowGraph_Click;


            stackPanel.Children.Add(diskBreakdownbutton);

            MAIN_AREA.Children.Add(stackPanel);

        }

        static private Tuple<double, string> give_appropriate_value(double value)
        {
            string type = "Byte";
            if (value >= 1024 * 1024 * 1024)
            {
                type = "GB";
                value = value / (1024 * 1024 * 1024);
            }
            else if (value >= 1024 * 1024)
            {
                type = "MB";
                value = value / (1024 * 1024);
            }
            else if (value >= 1024)
            {
                type = "KB";
                value = value / 1024;
            }

            return Tuple.Create(value, type);
        }

        private void ShowGraph_Click(object sender, RoutedEventArgs e)
        {
            // Get the data from the ResultTextBlock
            string data = ResultTextBlock.Text;
            ResultTextBlock.Text = "File Size";


            // Create the PieChart and configure it
            PieChart pieChart = new PieChart
            {
                Width = 400,
                Height = 400,
                Margin = new Thickness(10),
            };

            // Add the PieSeries to the chart
            SeriesCollection seriesCollection = new SeriesCollection();
            double total_value = 0;
            foreach (var kvp in categorySizes) total_value += kvp.Value;

            foreach (var kvp in categorySizes)
            {
                Tuple<double, string> app_value = give_appropriate_value(kvp.Value);

                if (kvp.Value > 0.049 * total_value)
                    seriesCollection.Add(new PieSeries
                    {
                        Title = kvp.Key,
                        Values = new ChartValues<double> { kvp.Value },
                        DataLabels = true,
                        LabelPoint = chartPoint => $"{kvp.Key}  :  {app_value.Item1:0.##} {app_value.Item2}", // Display value on the pie chart slice
                        LabelPosition = PieLabelPosition.InsideSlice // Display labels inside the slices to prevent overlapping
                    });
                else
                    seriesCollection.Add(new PieSeries
                    {
                        Title = kvp.Key,
                        Values = new ChartValues<double> { kvp.Value },
                        DataLabels = false,
                        LabelPoint = chartPoint => $"{kvp.Key}  :  {app_value.Item1:0.##} {app_value.Item2}",
                        LabelPosition = PieLabelPosition.InsideSlice // Display labels inside the slices to prevent overlapping
                    });
            }

            pieChart.Series = seriesCollection;

            // Create a StackPanel to hold the title and pie chart
            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(pieChart);

            // Clear the current content and add the StackPanel to the MAIN_AREA
            MAIN_AREA.Children.Add(stackPanel);
        }
        void ScanBtn_Click(object sender, RoutedEventArgs e)
        {
            // Clear any previous results
            if (ResultTextBlock != null)
            {
                MAIN_AREA.Children.RemoveAt(MAIN_AREA.Children.Count - 1);
                MAIN_AREA.Children.RemoveAt(MAIN_AREA.Children.Count - 1);
                ResultTextBlock.Text = null;
            }
            ResultTextBlock = new TextBlock
            {
                Text = "Scanning... ",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0) // Adjust the margin as needed
            };
            MAIN_AREA.Children.Add(ResultTextBlock);

            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string driveToScan = folderBrowserDialog.SelectedPath;


                if (string.IsNullOrEmpty(driveToScan))
                {
                    MessageBox.Show("Please enter a valid drive letter.");
                    return;
                }

                if (!Directory.Exists(driveToScan))
                {
                    MessageBox.Show("Drive not found or inaccessible.");
                    return;
                }

                string[] audioExtensions = { ".mp3", ".wav", ".m4a", ".flac", ".ogg", ".aac", ".wma", ".aiff", ".alac", ".amr", ".opus" };
                string[] videoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".m4v", ".mpeg", ".mpg", ".3gp", ".3g2" };
                string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif", ".webp", ".svg" };
                string[] documentExtensions = { ".doc", ".docx", ".pdf", ".txt", ".rtf", ".ppt", ".pptx", ".xls", ".xlsx", ".odt", ".ods", ".odp", ".csv" };
                string[] executableExtensions = { ".exe", ".dmg", ".app", ".bat", ".sh", ".msi", ".deb", ".rpm", ".jar", ".com", ".vbs", ".cmd" };

                var fileSizes = new ConcurrentDictionary<string, long>();

                double totalAudioSize = 0;
                double totalVideoSize = 0;
                double totalImageSize = 0;
                double totalExecutableSize = 0;
                double totalDocumentSize = 0;
                double totalOtherSize = 0;
                double total = 0;
                try
                {
                    foreach (string file in Directory.EnumerateFiles(driveToScan, "*.*", SearchOption.AllDirectories))
                    {
                        string extension = System.IO.Path.GetExtension(file).ToLower();
                        total += new FileInfo(file).Length;

                        if (audioExtensions.Contains(extension))
                        {
                            totalAudioSize += new FileInfo(file).Length;
                        }
                        else if (documentExtensions.Contains(extension))
                        {
                            totalDocumentSize += new FileInfo(file).Length;
                        }
                        else if (executableExtensions.Contains(extension))
                        {
                            totalExecutableSize += new FileInfo(file).Length;
                        }
                        else if (videoExtensions.Contains(extension))
                        {
                            totalVideoSize += new FileInfo(file).Length;
                        }
                        else if (imageExtensions.Contains(extension))
                        {
                            totalImageSize += new FileInfo(file).Length;
                        }
                        else
                        {
                            totalOtherSize += new FileInfo(file).Length;
                        }
                    }



                    // DICTIONARY PORTION

                    categorySizes["Audio"] = 1.0 * totalAudioSize;
                    categorySizes["Video"] = 1.0 * totalVideoSize;
                    categorySizes["Image"] = 1.0 * totalImageSize;
                    categorySizes["Document"] = 1.0 * totalDocumentSize;
                    categorySizes["Executable"] = 1.0 * totalExecutableSize;
                    categorySizes["Others"] = 1.0 * totalOtherSize;

                }
                catch (UnauthorizedAccessException)
                {
                    ResultTextBlock.Text = "Access denied. Run the application with appropriate permissions.";
                }
                catch (Exception ex)
                {
                    ResultTextBlock.Text = $"An error occurred: {ex.Message}";
                }
            }
        }

        // BUTTON 1 -> PROGRESS BAR FUNCTION
        public class ProgressBarWidthConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value is double width)
                {
                    return width * 0.8; // Set ProgressBar width to 80% of StackPanel width (you can adjust the percentage as needed)
                }
                return 0;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            createContent();
        }

    }

    public class Profile
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
    }
}
