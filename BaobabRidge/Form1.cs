using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BaobabRidge
{
    public partial class Form1 : Form
    {
        private readonly string filePath =
            Path.Combine(AppContext.BaseDirectory, "animals.txt");

        // Null means Add mode. Otherwise, the ID of the animal being edited.
        private string editingId = null;

        public Form1()
        {
            InitializeComponent();

            SetupAnimalGrid();
            LoadAnimals();

            dgvAnimals.CellClick += dgvAnimals_CellClick;
            btnSave.Click += btnSave_Click;
            btnClear.Click += btnClear_Click;

            // Start in Add mode.
            SetEditMode(false);
        }

        // Configures the grid columns and prevents direct editing.
        private void SetupAnimalGrid()
        {
            dgvAnimals.Columns.Clear();
            dgvAnimals.AutoGenerateColumns = false;

            dgvAnimals.ReadOnly = true;
            dgvAnimals.AllowUserToAddRows = false;
            dgvAnimals.AllowUserToDeleteRows = false;
            dgvAnimals.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvAnimals.MultiSelect = false;
            dgvAnimals.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            AddGridColumn("AnimalID", "Animal ID");
            AddGridColumn("Name", "Name");
            AddGridColumn("Species", "Species");
            AddGridColumn("Age", "Age");
            AddGridColumn("RecoveryScore", "Recovery Score");
        }

        // Adds a read-only column with the correct heading.
        private void AddGridColumn(string property, string heading)
        {
            dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = property,
                Name = property,
                HeaderText = heading,
                ReadOnly = true
            });
        }

        // Reads animal records from animals.txt.
        // 5. File reading method
        private List<Animal> ReadAnimals(out int skippedLines)
        {
            List<Animal> animals = new List<Animal>();
            skippedLines = 0;

            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { }
                return animals;
            }

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            foreach (string line in lines)
            {
                string[] fields = line.Split('|');

                if (fields.Length != 7)
                {
                    skippedLines++;
                    continue;
                }

                string id = fields[0].Trim().ToUpperInvariant();

                bool validID = Regex.IsMatch(
                    id, @"^WR-[0-9]{4}$");

                bool validAge = int.TryParse(
                    fields[3], out int age);

                bool validScore = int.TryParse(
                    fields[4], out int score);

                if (!validID || !validAge || !validScore ||
                    age < 0 || age > 100 ||
                    score < 0 || score > 100)
                {
                    skippedLines++;
                    continue;
                }

                Animal animal = new Animal
                {
                    AnimalID = id,
                    Name = fields[1],
                    Species = fields[2],
                    Age = age,
                    RecoveryScore = score
                };

                animals.Add(animal);
            }

            return animals;
        }

        // Loads animal records into the DataGridView.
        // 6. Grid loading method
        public void LoadAnimals()
        {
            try
            {
                int skippedLines;

                List<Animal> animals =
                    ReadAnimals(out skippedLines);

                dgvAnimals.DataSource = null;
                dgvAnimals.DataSource = animals;

                dgvAnimals.ClearSelection();

                if (skippedLines > 0)
                {
                    MessageBox.Show(
                        skippedLines +
                        " invalid record(s) were skipped.",
                        "File Loading",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "Permission denied when accessing animals.txt.",
                    "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "Unable to access animals.txt. " +
                    "The file may be in use.",
                    "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Controls which buttons and fields are enabled.
        private void SetEditMode(bool edit)
        {
            txtAnimalId.ReadOnly = edit;
            btnAdd.Enabled = !edit;
            btnSave.Enabled = edit;
        }

        // Clears the form and returns to Add mode.
        private void ClearForm()
        {
            editingId = null;

            txtAnimalId.Clear();
            txtName.Clear();
            txtSpecies.Clear();
            txtAge.Clear();
            txtRecoveryScore.Clear();

            SetEditMode(false);
            txtAnimalId.Focus();
        }

        // Retained in case other code still calls this method.
        private void ClearInputs()
        {
            ClearForm();
        }

        // Loads the selected animal into the form for editing.
        private void LoadForEdit(Animal a)
        {
            editingId = a.AnimalID;

            txtAnimalId.Text = a.AnimalID;
            txtName.Text = a.Name;
            txtSpecies.Text = a.Species;
            txtAge.Text = a.Age.ToString();
            txtRecoveryScore.Text = a.RecoveryScore.ToString();

            SetEditMode(true);
        }

        // Loads an animal when the user clicks a row.
        private void dgvAnimals_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgvAnimals.Rows[e.RowIndex].DataBoundItem
                is Animal a)
            {
                LoadForEdit(a);
            }
        }

        // Searches for an animal by ID.
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string id = txtSearchId.Text.Trim();

            if (id.Length == 0)
            {
                MessageBox.Show(
                    "Please enter an Animal ID to search for.");
                return;
            }

            try
            {
                List<Animal> animals = ReadAnimals(out _);

                Animal match = animals.FirstOrDefault(a =>
                    a.AnimalID.Equals(
                        id, StringComparison.OrdinalIgnoreCase));

                if (match == null)
                {
                    MessageBox.Show(
                        "No animal found with ID \"" + id + "\".",
                        "Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                LoadForEdit(match);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied.");
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "Unable to read animals.txt. The file may be in use.");
            }
        }

        // Adds a new animal.
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!AnimalValidator.TryCreateAnimal(
                txtAnimalId.Text,
                txtName.Text,
                txtSpecies.Text,
                txtAge.Text,
                txtRecoveryScore.Text,
                out Animal animal,
                out string errorField,
                out string errorMessage))
            {
                MessageBox.Show(
                    errorMessage,
                    "Invalid input",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                FocusField(errorField);
                return;
            }

            if (FileHandler.IdExists(animal.AnimalID))
            {
                MessageBox.Show(
                    $"Animal ID {animal.AnimalID} already exists.",
                    "Duplicate ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtAnimalId.Focus();
                txtAnimalId.SelectAll();
                return;
            }

            if (!FileHandler.TryAppendAnimal(
                animal, out string saveError))
            {
                MessageBox.Show(
                    saveError,
                    "Could not save",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            MessageBox.Show(
                $"{animal.AnimalID} added.\n" +
                $"Status: {animal.Status}\n" +
                $"Housing Unit: {animal.HousingUnit}",
                "Animal added",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LoadAnimals();
            ClearForm();
        }

        // Moves the cursor to the field that failed validation.
        private void FocusField(string fieldName)
        {
            TextBox target;

            switch (fieldName)
            {
                case "Animal ID":
                    target = txtAnimalId;
                    break;

                case "Name":
                    target = txtName;
                    break;

                case "Species":
                    target = txtSpecies;
                    break;

                case "Age":
                    target = txtAge;
                    break;

                default:
                    target = txtRecoveryScore;
                    break;
            }

            target.Focus();
            target.SelectAll();
        }

        // Updates the selected animal.
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (editingId == null)
            {
                MessageBox.Show(
                    "Please select an animal to update.");
                return;
            }

            // Use the same validation rules as Add.
            if (!AnimalValidator.TryCreateAnimal(
                txtAnimalId.Text,
                txtName.Text,
                txtSpecies.Text,
                txtAge.Text,
                txtRecoveryScore.Text,
                out Animal animal,
                out string errorField,
                out string errorMessage))
            {
                MessageBox.Show(
                    errorMessage,
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                FocusField(errorField);
                return;
            }

            // Keep the original ID and use the validated animal details.
            string newLine = string.Join("|",
                editingId,
                animal.Name,
                animal.Species,
                animal.Age,
                animal.RecoveryScore,
                animal.Status,
                animal.HousingUnit);

            string tempPath = filePath + ".tmp";

            try
            {
                string[] lines =
                    File.ReadAllLines(filePath, Encoding.UTF8);

                int index = -1;

                // Find the existing record.
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] fields = lines[i].Split('|');

                    if (fields.Length > 0 &&
                        fields[0].Trim().Equals(
                            editingId,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                {
                    MessageBox.Show(
                        "This animal no longer exists in the file.");

                    LoadAnimals();
                    ClearForm();
                    return;
                }

                // Replace only the selected record.
                lines[index] = newLine;

                // Write the updated records to a temporary file.
                File.WriteAllLines(tempPath, lines, Encoding.UTF8);

                // Swap the temporary file in for the original.
                File.Replace(tempPath, filePath, null);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "Permission denied when saving animals.txt.");
                return;
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "Animals.txt could not be read or saved. " +
                    "The file may be in use.");

                return;
            }
            finally
            {
                // Remove a leftover temporary file if one exists.
                try
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                }
                catch (IOException)
                {
                    // The next save can handle the leftover file.
                }
                catch (UnauthorizedAccessException)
                {
                    // Do not hide the original operation's result.
                }
            }

            MessageBox.Show(
                "Animal updated successfully.",
                "Update Successful",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LoadAnimals();
            ClearForm();
        }

        // Clear button.
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        // Refresh button.
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAnimals();
        }

        // Deletes the selected animal from the file.
        public void DeleteAnimal(string animalID, string animalName)
        {
            DialogResult answer = MessageBox.Show(
                "Delete " + animalID + " (" + animalName +
                ")? This cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
                return;

            try
            {
                List<string> lines =
                    File.ReadAllLines(filePath, Encoding.UTF8).ToList();

                int removed = lines.RemoveAll(line =>
                {
                    string[] fields = line.Split('|');

                    return fields.Length > 0 &&
                        fields[0].Trim().Equals(
                            animalID,
                            StringComparison.OrdinalIgnoreCase);
                });

                if (removed == 0)
                {
                    MessageBox.Show("Animal not found.");
                    LoadAnimals();
                    return;
                }

                File.WriteAllLines(filePath, lines, Encoding.UTF8);

                // If the deleted animal is the one being edited,
                // leave edit mode and clear the form.
                if (editingId != null &&
                    editingId.Equals(
                        animalID, StringComparison.OrdinalIgnoreCase))
                {
                    ClearForm();
                }

                LoadAnimals();

                MessageBox.Show(
                    "Animal deleted successfully.");
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "Animals.txt could not be read or saved. " +
                    "The file may be open in another program.");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied.");
            }
        }

        // Deletes the selected animal after confirmation.
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAnimals.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an animal first.");
                return;
            }

            string id = dgvAnimals.SelectedRows[0]
                .Cells["AnimalID"].Value?.ToString() ?? "";

            string name = dgvAnimals.SelectedRows[0]
                .Cells["Name"].Value?.ToString() ?? "";

            if (id.Length == 0)
            {
                MessageBox.Show("The selected animal has no valid ID.");
                return;
            }

            DeleteAnimal(id, name);
        }

        private void btnGenerateSummary_Click(object sender, EventArgs e)
        {
            List<Animal> animals = FileHandler.LoadAnimals(out int skipped, out string loadError);
            if (animals == null)
            {
                MessageBox.Show(loadError, "Could not read animals", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SummaryReport report = SummaryReport.Create(animals);

            lblTotalValue.Text = report.Total.ToString();
            lblAverageAgeValue.Text = report.AverageAgeText;
            lblAverageScoreValue.Text = report.AverageScoreText;
            lblCriticalValue.Text = report.StatusCounts["Critical"].ToString();
            lblSeriousValue.Text = report.StatusCounts["Serious"].ToString();
            lblStableValue.Text = report.StatusCounts["Stable"].ToString();
            lblRecoveringValue.Text = report.StatusCounts["Recovering"].ToString();
            lblReleaseReadyValue.Text = report.StatusCounts["Release-Ready"].ToString();

            if (!FileHandler.TryWriteSummary(report.ToReportText(DateTime.Now), out string error))
                MessageBox.Show(error, "Could not save summary", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("Summary saved to summary.txt.", "Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            string id = txtSearchId.Text.Trim();

            if (id.Length == 0)
            {
                MessageBox.Show(
                    "Please enter an Animal ID to search for.");
                return;
            }

            try
            {
                List<Animal> animals = ReadAnimals(out _);

                Animal match = animals.FirstOrDefault(a =>
                    a.AnimalID.Equals(
                        id, StringComparison.OrdinalIgnoreCase));

                if (match == null)
                {
                    MessageBox.Show(
                        "No animal found with ID \"" + id + "\".",
                        "Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                LoadForEdit(match);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied.");
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "Unable to read animals.txt. The file may be in use.");
            }
        }
    }
    
}