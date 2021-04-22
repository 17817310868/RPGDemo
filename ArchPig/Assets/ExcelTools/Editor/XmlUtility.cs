using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Text;
using Excel;
using System.Data;
using OfficeOpenXml;

public class XmlUtility  {

    private DataSet mResultSet;

    public string[] Col1;
    public string[] Col2;
    public string a;
    public XmlUtility()
    {

    }

    /// <summary>
    /// 转换为xlsxs
    /// </summary>
    public void ConvertToXLSX(string CSVPath,string xmlpath)
    {
        Debug.Log("CSVPath"+CSVPath);
        Debug.Log("xmlpath"+xmlpath);



        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlpath);


        //*******************获取root节点下的子节点名称****************************//

        int rootlist = xmlDoc.GetElementsByTagName("root")[0].ChildNodes.Count;
         string NextNode=" ";

        for (int k = 0; k < rootlist;k++)
        {
            NextNode = xmlDoc.GetElementsByTagName("root")[0].ChildNodes[k].Name;
            a = NextNode;
            Debug.Log(a);
            break;
        }
     



        //*********************************************//


        int nodeCount = xmlDoc.GetElementsByTagName(NextNode)[0].ChildNodes.Count;
        XmlNodeList nodelist = xmlDoc.GetElementsByTagName(NextNode);

        //Debug.Log(nodeCount.ToString());

       
        FileStream fs = new FileStream(CSVPath, FileMode.Create);
        using (var package = new ExcelPackage(fs))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");

            //worksheet.Cells[1, 1].Value = "1";
            for (int i = 0; i < nodelist.Count; i++)
            {
                Debug.Log(i.ToString());
                for (int j = 0; j < nodeCount; j++)
                {
                    Debug.Log(j.ToString());
                    //这行主要插入表头，即自己点名称
                    string item = xmlDoc.GetElementsByTagName(NextNode)[i].ChildNodes[j].InnerText;
                    string value = item;
                    //每列加一，
                    int a = j + 1;
                    //因为遥空出一行给表头，所以加2
                    int b = i + 2;
                    //这行主要插入表头，即自己点名称
                    worksheet.Cells[1, a].Value = xmlDoc.GetElementsByTagName(NextNode)[i].ChildNodes[j].Name;
                    //这行主要插入自己点的数据
                    worksheet.Cells[b, a].Value = value;

                }
            }
            

            package.Save();

        }



    }
}
