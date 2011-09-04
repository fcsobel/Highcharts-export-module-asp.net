using System;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Svg;

namespace highchart_export_module_asp_net
{
    public partial class highcharts_export : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.Form["type"] != null && Request.Form["svg"] != null && Request.Form["filename"] != null)
                {
                    string tType = Request.Form["type"].ToString();
                    string tSvg = Request.Form["svg"].ToString();
                    string tFileName = Request.Form["filename"].ToString();
                    if (tFileName == "") tFileName = "chart";

                    MemoryStream data = new MemoryStream(Encoding.ASCII.GetBytes(tSvg));
                    MemoryStream stream = new MemoryStream();

                    switch (tType)
                    {
                        case "image/png":
                            tFileName += ".png";
                            SvgDocument.Open(data).Draw().Save(stream, ImageFormat.Png);
                            break;
                        case "image/jpeg":
                            tFileName += ".jpg";
                            SvgDocument.Open(data).Draw().Save(stream, ImageFormat.Jpeg);
                            break;
                        case "image/svg+xml":
                            tFileName += ".svg";
                            stream = data;
                            break;
                        case "application/pdf":
                            tFileName += ".pdf";

                            PdfWriter tWriter = null;
                            Document tDocumentPdf = null;
                            try
                            {
                                SvgDocument tSvgObj = SvgDocument.Open(data);

                                tSvgObj.Draw().Save(stream, ImageFormat.Png);

                                // Creating pdf document
                                tDocumentPdf = new Document(new iTextSharp.text.Rectangle((float)tSvgObj.Width, (float)tSvgObj.Height));

                                // setting up margin to full screen image
                                tDocumentPdf.SetMargins(0.0f, 0.0f, 0.0f, 0.0f);

                                // creating image
                                iTextSharp.text.Image tGraph = iTextSharp.text.Image.GetInstance(stream.ToArray());
                                tGraph.ScaleToFit((float)tSvgObj.Width, (float)tSvgObj.Height);

                                stream = new MemoryStream();

                                // create writer
                                tWriter = PdfWriter.GetInstance(tDocumentPdf, stream);

                                tDocumentPdf.Open();
                                tDocumentPdf.NewPage();
                                tDocumentPdf.Add(tGraph);
                                tDocumentPdf.CloseDocument();
                                tDocumentPdf.Close();
                            }
                            catch (Exception ex)
                            {
                                throw ex;

                            }
                            finally
                            {
                                tDocumentPdf.Close();
                                tDocumentPdf.Dispose();
                                tWriter.Close();
                                tWriter.Dispose();
                            }
                            break;
                    }

                    if (stream != null)
                    {
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.ContentType = tType;
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" + tFileName);
                        Response.BinaryWrite(stream.ToArray());
                        Response.End();
                    }
                }
            }
        }
    }
}
