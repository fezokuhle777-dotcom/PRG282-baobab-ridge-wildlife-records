using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaobabRidge
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();

            SetupAnimalGrid();
            LoadAnimals(); 


            dgvAnimals.CellClick += dgvAnimals_CellClick;
            btnSave.Click += btnSave_Click;
            btnClear.Click += btnClear_Click;
            SetEditMode(false);   // start in Add mode
           
    }


        private readonly string filePath =
        Path.Combine(AppContext.BaseDirectory, "animals.txt");

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

        // 5. File reading method
        private List<Animals> ReadAnimals(out int skippedLines)
        {
            List<Animals> animals = new List<Animals>();
            skippedLines = 0;

            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { }
                return animals;
            }

            string[] lines = File.ReadAllLines(
                filePath, Encoding.UTF8);

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

                Animals animal = new Animals
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

        // 6. Grid loading method
        public void LoadAnimals()
        {
            try
            {
                int skippedLines;

                List<Animals> animals =
                    ReadAnimals(out skippedLines);

                // Clear the old data before displaying new records.
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

        // Deletes the selected animal from the file.
        public void DeleteAnimal(string animalID, string animalName)
        {
            DialogResult answer = MessageBox.Show(
                "Delete " + animalID + " (" + animalName +
                ")? This cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes) return;

            try
            {
                List<string> lines = File.ReadAllLines(filePath, Encoding.UTF8).ToList();

                int removed = lines.RemoveAll(line =>
                    line.Split('|')[0].Trim().Equals(
                        animalID, StringComparison.OrdinalIgnoreCase));

                if (removed == 0)
                {
                    MessageBox.Show("Animal not found.");
                    LoadAnimals();
                    return;
                }

                File.WriteAllLines(filePath, lines, Encoding.UTF8);

                LoadAnimals();

                MessageBox.Show("Animal deleted successfully.");
            }
            catch (IOException)
            {
                MessageBox.Show("Animals.txt could not be read or saved. The file may be open in another program.");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied.");
            }
        }

        // Refreshes the animal grid.
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAnimals();
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
                .Cells["AnimalID"].Value.ToString();

            string name = dgvAnimals.SelectedRows[0]
                .Cells["Name"].Value.ToString();

            DeleteAnimal(id, name);
        }

        // null = Add mode; otherwise the ID of the record being edited.
        private string? editingId = null;




        // 3.2 Edit mode: ID read-only, Add disabled, Save Changes enabled.
        private void SetEditMode(bool edit)
        {
            txtAnimalID.ReadOnly = edit;
            btnAdd.Enabled = !edit;
            btnSave.Enabled = edit;
        }

        // 3.3 Leave edit mode and return to Add mode with empty boxes.
        private void ClearForm()
        {
            editingId = null;
            txtAnimalID.Clear();
            txtName.Clear();
            txtSpecies.Clear();
            txtAge.Clear();
            txtRecoveryScore.Clear();
            SetEditMode(false);
        }

        // 3.1 Both load methods put the record into the same input boxes.
        private void LoadForEdit(Animals a)
        {
            editingId = a.AnimalID;
            txtAnimalID.Text = a.AnimalID;
            txtName.Text = a.Name;
            txtSpecies.Text = a.Species;
            txtAge.Text = a.Age.ToString();
            txtRecoveryScore.Text = a.RecoveryScore.ToString();
            SetEditMode(true);
        }

        // 3.1 Load by clicking a row in the grid.
        private void dgvAnimals_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;   // header click

            if (dgvAnimals.Rows[e.RowIndex].DataBoundItem is Animals a)
                LoadForEdit(a);
        }

        // 3.1 Load by Animal ID (exact match, not case-sensitive).
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string id = txtSearch.Text.Trim();

            if (id.Length == 0)
            {
                MessageBox.Show("Please enter an Animal ID to search for.");
                return;
            }

            try
            {
                List<Animals> animals = ReadAnimals(out _);

                Animals? match = animals.FirstOrDefault(a =>
                    a.AnimalID.Equals(id, StringComparison.OrdinalIgnoreCase));

                if (match == null)
                {
                    MessageBox.Show(
                        "No animal found with ID \"" + id + "\".",
                        "Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;   // nothing else changes
                }

                LoadForEdit(match);
            }
            catch (IOException)
            {
                MessageBox.Show("Unable to read animals.txt. The file may be in use.");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied.");
            }
        }

        // 3.3 Clear button.
        private void btnClear_Click(object? sender, EventArgs e)
        {
            ClearForm();
        }

        // 3.4 Same rules as Add, except the ID is not re-checked for uniqueness.
        // If your Add feature already has a validation method, call that instead.
        private bool ValidateAnimalInputs(out string error)
        {
            error = "";

            string name = txtName.Text.Trim();
            string species = txtSpecies.Text.Trim();

            if (name.Length == 0)
            {
                error = "Name is required.";
                return false;
            }

            if (species.Length == 0)
            {
                error = "Species is required.";
                return false;
            }

            // '|' is the file delimiter, so it would corrupt the record.
            if (name.Contains('|') || species.Contains('|'))
            {
                error = "Name and Species cannot contain the '|' character.";
                return false;
            }

            if (!int.TryParse(txtAge.Text.Trim(), out int age) || age < 0 || age > 100)
            {
                error = "Age must be a whole number from 0 to 100.";
                return false;
            }

            if (!int.TryParse(txtRecoveryScore.Text.Trim(), out int score) || score < 0 || score > 100)
            {
                error = "Recovery Score must be a whole number from 0 to 100.";
                return false;
            }

            return true;
        }

        // PLACEHOLDERS: replace with your real rules. If your Add feature already
        // has these methods, delete these two and use yours.
        private static string CalculateStatus(int score)
        {
            if (score < 30) return "Critical";
            if (score < 60) return "Recovering";
            return "Stable";
        }

        private static string CalculateHousingUnit(int score)
        {
            if (score < 30) return "Intensive Care";
            if (score < 60) return "Rehabilitation";
            return "Outdoor Enclosure";
        }

        // 3.4 / 3.5 Save Changes.
        private void btnSave_Click(object? sender, EventArgs e)
        {
            if (editingId == null) return;

            if (!ValidateAnimalInputs(out string error))
            {
                MessageBox.Show(error, "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int age = int.Parse(txtAge.Text.Trim());
            int score = int.Parse(txtRecoveryScore.Text.Trim());

            // Status and housing unit are always recalculated from the score,
            // even if the score is unchanged.
            string newLine = string.Join("|",
                editingId,
                txtName.Text.Trim(),
                txtSpecies.Text.Trim(),
                age,
                score,
                CalculateStatus(score),
                CalculateHousingUnit(score));

            try
            {
                string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

                // Find the one line belonging to this animal.
                int index = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Split('|')[0].Trim().Equals(
                            editingId, StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                        break;
                    }
                }

                if (index < 0)
                {
                    MessageBox.Show("This animal no longer exists in the file.");
                    LoadAnimals();
                    ClearForm();
                    return;
                }

                // Only this one line changes; every other line is written back as-is,
                // so no other record can be modified, lost or duplicated.
                lines[index] = newLine;

                // Write to a temp file first so a failure cannot corrupt animals.txt.
                string tempPath = filePath + ".tmp";
                File.WriteAllLines(tempPath, lines, Encoding.UTF8);
                File.Move(tempPath, filePath, true);
            }
            catch (IOException)
            {
                MessageBox.Show("Animals.txt could not be read or saved. The file may be open in another program.");
                return;   // stay in edit mode so the user's changes are not lost
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied.");
                return;
            }

            // 3.5 Grid refreshes and the form returns to Add mode.
            LoadAnimals();
            ClearForm();
            MessageBox.Show("Animal updated successfully.");
        }
    }
}

