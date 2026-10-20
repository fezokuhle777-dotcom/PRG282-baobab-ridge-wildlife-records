# PRG282-baobab-ridge-wildlife-records
C# Windows Forms app for Baobab Ridge Wildlife Rehabilitation Centre: records animals, calculates status and housing from a Recovery Score, and generates summary reports. Text-file storage, Git and GitHub. PRG282 project.

# Baobab Ridge Wildlife Rehabilitation Records System

Project PRG2782, Belgium Campus.
A version-controlled C# Windows Forms application that records animals in care at the (fictional) Baobab Ridge Wildlife Rehabilitation Centre. Data is stored in plain text files and the code is managed with Git and GitHub.

## Group members

- [Reamogetswe Modise] - [603348] - Feature 1: Add, Feature 5: Summary Report
- [Fezokuhle Sibeko] - [603460] - Feature 3: Update 
- [Angel Nkosazana Mthenjane ] - [603995] - Feature 2: View
- [Mukudzeyi tawan Fundira] - [604998] - Feature 4: Delete

## How to build and run

1. Install Visual Studio 2022 with the ".NET desktop development" workload.
2. Clone the repository: `git clone https://github.com/fezokuhle777-dotcom/PRG282-baobab-ridge-wildlife-records.git`
3. Open the `.sln` file in Visual Studio.
4. Set the configuration to Debug and press F5 (or Ctrl + F5) to build and run.
5. `animals.txt` is copied beside the .exe on build ("Copy if newer"), so no extra setup is needed.

Target framework: [.NET 8 / write whatever Project > Properties > Application shows]

## Features

- **Add** - Validates Animal ID, Name, Species, Age and Recovery Score, calculates the status and housing unit, and appends the animal to `animals.txt`. Shows the calculated status and housing unit.
- **View** - Loads `animals.txt` into a read-only grid on startup and refreshes after every add, update and delete. Malformed lines are skipped and counted.
- **Update** - Loads an animal by clicking a row or searching by Animal ID (not case-sensitive). The ID cannot be changed. Status and housing unit are recalculated on save, and only the edited record changes.
- **Delete** - Deletes the selected animal after a Yes/No confirmation that shows the ID and name.
- **Summary Report** - Calculates the total, average age, average recovery score and the count per status. Results appear on the form and are saved to `summary.txt`.

Classification is done in one method (`AnimalClassifier.Classify`), which Add and Update both use.

## File formats

**animals.txt** (beside the .exe): one animal per line, seven fields separated by `|`, with no header and no blank lines, in UTF-8:

    ID|Name|Species|Age|Score|Status|Housing Unit
    WR-0001|Thabo|African Penguin|4|85|Release-Ready|Pre-Release Camp

**summary.txt** (beside the .exe, overwritten each time the summary is generated): a title, a timestamp (`yyyy-MM-dd HH:mm`), the total, the average age and the average recovery score (two decimal places, `N/A` when there are no animals), then a count for each of the five statuses (Critical, Serious, Stable, Recovering, Release-Ready).

## Known limitations

- Reading `animals.txt` is done in two places (`Form1.ReadAnimals` for the grid and `FileHandler.LoadAnimals` for the summary). They should be merged into one method.
- Only one animal can be deleted at a time.
- The whole file is read each time the grid refreshes, which is fine for small files but not for very large ones.
- [Add anything else your group knows about.]
