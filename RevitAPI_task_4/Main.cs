using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevitAPI_task_4
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            string wallInfo = string.Empty;

            var walls = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .Cast<Wall>()
                .ToList();

            foreach (Wall wall in walls)
            {
                double wallVolumeValue = 0;
                Parameter wallVolumeParameter = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                if (wallVolumeParameter.StorageType == StorageType.Double)
                {
                    wallVolumeValue = UnitUtils.ConvertFromInternalUnits(wallVolumeParameter.AsDouble(), UnitTypeId.CubicMeters);
                }

                wallInfo += $"{wall.WallType.Name}\t{wallVolumeValue}{Environment.NewLine}";
            }

            var saveDialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "All files (*.*)|*.*",
                FileName = "wallInfo.txt",
                DefaultExt = ".txt"
            };

            string selectedFilePath = string.Empty;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = saveDialog.FileName;
            }

            if (string.IsNullOrEmpty(selectedFilePath))
                return Result.Cancelled;

            File.WriteAllText(selectedFilePath, wallInfo);


            return Result.Succeeded;
        }
    }
}
