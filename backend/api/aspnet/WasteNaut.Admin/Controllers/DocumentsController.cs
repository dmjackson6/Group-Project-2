using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace WasteNaut.Admin.Controllers
{
    [ApiController]
    public class DocumentsController : ControllerBase
    {

        [HttpGet("/resources/{filename}")]
        public IActionResult GetResource(string filename)
        {
            // Generate HTML content
            var html = filename.ToLower() switch
            {
                "food-safety-guide.pdf" => GenerateFoodSafetyGuideHtml(),
                "donation-checklist.pdf" => GenerateDonationChecklistHtml(),
                _ => GenerateGenericDocumentHtml(filename)
            };

            // Convert HTML to PDF
            var pdfBytes = ConvertHtmlToPdf(html);
            
            // Force download
            return File(pdfBytes, "application/pdf", filename);
        }

        [HttpGet("/receipts/{filename}")]
        public IActionResult GetReceipt(string filename)
        {
            // Parse filename: donorId-taxYear.pdf
            // Handle donorIds that may contain dashes by splitting from the end
            var nameWithoutExtension = filename.Replace(".pdf", "").Replace(".PDF", "");
            var lastDashIndex = nameWithoutExtension.LastIndexOf('-');
            
            string donorId;
            string taxYear;
            
            if (lastDashIndex > 0 && lastDashIndex < nameWithoutExtension.Length - 1)
            {
                // Split at the last dash: everything before is donorId, everything after is taxYear
                var potentialTaxYear = nameWithoutExtension.Substring(lastDashIndex + 1);
                
                // Validate taxYear is numeric (4 digits typically)
                if (System.Text.RegularExpressions.Regex.IsMatch(potentialTaxYear, @"^\d{4}$"))
                {
                    // Valid 4-digit year found - use it
                    donorId = nameWithoutExtension.Substring(0, lastDashIndex);
                    taxYear = potentialTaxYear;
                }
                else
                {
                    // Not a valid year format - assume the whole thing is donorId and use current year
                    donorId = nameWithoutExtension;
                    taxYear = DateTime.Now.Year.ToString();
                }
            }
            else
            {
                // No dash found or malformed filename
                donorId = nameWithoutExtension;
                taxYear = DateTime.Now.Year.ToString();
            }

            // Generate tax receipt PDF directly (tables need special handling)
            var pdfBytes = GenerateTaxReceiptPdf(donorId, taxYear);
            
            // Force download
            return File(pdfBytes, "application/pdf", filename);
        }

        [HttpGet("/debug/pdf-test")]
        public IActionResult DebugPdfTest()
        {
            // Create a simple test PDF to verify the structure works
            var testLines = new List<PdfLine>
            {
                new PdfLine { Text = "Test Document", IsTitle = true, FontSize = 22 },
                new PdfLine { Text = "WasteNaut Food Rescue Platform", IsBold = true, FontSize = 12 },
                new PdfLine { Text = "This is a test line to verify PDF generation is working.", FontSize = 11 },
                new PdfLine { Text = "If you can see this text, the PDF is rendering correctly.", FontSize = 11 },
                new PdfLine { Text = "Section 1", IsSectionHeading = true, FontSize = 14 },
                new PdfLine { Text = "Item 1: This should appear on the page", FontSize = 11 },
                new PdfLine { Text = "Item 2: This should also appear", FontSize = 11 },
                new PdfLine { Text = "Item 3: And this too", FontSize = 11 },
                new PdfLine { Text = "Section 2", IsSectionHeading = true, FontSize = 14 },
                new PdfLine { Text = "More content here to test page breaks", FontSize = 11 }
            };
            
            // Add many lines to force a second page
            for (int i = 0; i < 50; i++)
            {
                testLines.Add(new PdfLine { Text = $"Line {i + 1}: This is additional content to test multi-page PDF generation.", FontSize = 11 });
            }
            
            var pdfBytes = CreateFormattedPdf(testLines);
            return File(pdfBytes, "application/pdf", "test.pdf");
        }

        [HttpGet("/debug/pdf-raw/{docType}")]
        public IActionResult DebugPdfRaw(string docType)
        {
            string html = docType.ToLower() switch
            {
                "food" or "food-safety" => GenerateFoodSafetyGuideHtml(),
                "checklist" or "donation" => GenerateDonationChecklistHtml(),
                "receipt" or "tax" => GenerateTaxReceiptHtml("test-donor", "2024"),
                _ => GenerateGenericDocumentHtml("test")
            };
            
            var lines = ExtractStructuredContent(html);
            
            // Return info about what will be rendered
            var info = new
            {
                totalLines = lines.Count,
                pagesExpected = (int)Math.Ceiling(lines.Count / 25.0), // Rough estimate
                firstPageLines = lines.Take(25).Select((l, i) => new 
                { 
                    index = i,
                    text = l.Text.Length > 60 ? l.Text.Substring(0, 60) + "..." : l.Text,
                    type = l.IsTitle ? "Title" : (l.IsSectionHeading ? "H2" : (l.IsHeading ? "H1" : (l.IsBold ? "Bold" : "Text"))),
                    fontSize = l.FontSize
                }).ToList()
            };
            
            return new JsonResult(info);
        }

        [HttpGet("/debug/extract-content/{docType}")]
        public IActionResult DebugExtractContent(string docType)
        {
            string html = docType.ToLower() switch
            {
                "food" or "food-safety" => GenerateFoodSafetyGuideHtml(),
                "checklist" or "donation" => GenerateDonationChecklistHtml(),
                "receipt" or "tax" => GenerateTaxReceiptHtml("test-donor", "2024"),
                _ => GenerateGenericDocumentHtml("test")
            };
            
            var lines = ExtractStructuredContent(html);
            
            var result = new
            {
                htmlLength = html.Length,
                linesExtracted = lines.Count,
                lines = lines.Select(l => new
                {
                    text = l.Text,
                    isHeading = l.IsHeading,
                    isBold = l.IsBold,
                    isTitle = l.IsTitle,
                    isSectionHeading = l.IsSectionHeading,
                    fontSize = l.FontSize
                }).ToList()
            };
            
            return new JsonResult(result);
        }

        [HttpGet("/debug/pdf-content/{docType}")]
        public IActionResult DebugPdfContent(string docType)
        {
            string html = docType.ToLower() switch
            {
                "food" or "food-safety" => GenerateFoodSafetyGuideHtml(),
                "checklist" or "donation" => GenerateDonationChecklistHtml(),
                "receipt" or "tax" => GenerateTaxReceiptHtml("test-donor", "2024"),
                _ => GenerateGenericDocumentHtml("test")
            };
            
            var lines = ExtractStructuredContent(html);
            var pdfBytes = CreateFormattedPdf(lines);
            
            var debug = new
            {
                linesExtracted = lines.Count,
                first10Lines = lines.Take(10).Select(l => new { text = l.Text.Substring(0, Math.Min(50, l.Text.Length)), isTitle = l.IsTitle, isHeading = l.IsHeading, isSectionHeading = l.IsSectionHeading }).ToList(),
                pdfSize = pdfBytes.Length,
                pdfHeader = System.Text.Encoding.ASCII.GetString(pdfBytes.Take(50).ToArray())
            };
            
            return new JsonResult(debug);
        }

        [HttpGet("/debug/test-direct-pdf")]
        public IActionResult TestDirectPdf()
        {
            // Bypass all extraction - render content directly like tax receipt does with tables
            var directLines = new List<PdfLine>
            {
                new PdfLine { Text = "Food Safety Guidelines", IsTitle = true, FontSize = 22 },
                new PdfLine { Text = "WasteNaut Food Rescue Platform", IsBold = true, FontSize = 12 },
                new PdfLine { Text = "1. TEMPERATURE CONTROL", IsSectionHeading = true, FontSize = 14 },
                new PdfLine { Text = "- Perishable items must be kept at or below 40 degF", FontSize = 11 },
                new PdfLine { Text = "- Hot foods must be kept at or above 140 degF", FontSize = 11 },
                new PdfLine { Text = "- Use thermometers to monitor temperatures", FontSize = 11 },
                new PdfLine { Text = "2. STORAGE GUIDELINES", IsSectionHeading = true, FontSize = 14 },
                new PdfLine { Text = "- Store items off the floor on clean shelving", FontSize = 11 },
                new PdfLine { Text = "- Separate raw and cooked items", FontSize = 11 },
            };
            
            var pdfBytes = CreateFormattedPdf(directLines);
            return File(pdfBytes, "application/pdf", "test-direct.pdf");
        }

        private byte[] ConvertHtmlToPdf(string html)
        {
            // Extract structured content from HTML
            var lines = ExtractStructuredContent(html);
            
            // Create formatted PDF using QuestPDF
            var pdfBytes = CreateFormattedPdf(lines);
            return pdfBytes;
        }
        
        private List<PdfLine> ExtractStructuredContent(string html)
        {
            var lines = new List<PdfLine>();
            
            // Remove script and style tags completely
            html = System.Text.RegularExpressions.Regex.Replace(html, @"<script[^>]*>.*?</script>", "", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            html = System.Text.RegularExpressions.Regex.Replace(html, @"<style[^>]*>.*?</style>", "", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            html = System.Text.RegularExpressions.Regex.Replace(html, @"<meta[^>]*>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            html = System.Text.RegularExpressions.Regex.Replace(html, @"<title[^>]*>.*?</title>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            // Extract document title from first h1
            var h1Match = System.Text.RegularExpressions.Regex.Match(html, @"<h1[^>]*>(.*?)</h1>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (h1Match.Success)
            {
                var title = CleanHtmlText(h1Match.Groups[1].Value);
                if (!string.IsNullOrWhiteSpace(title))
                    lines.Add(new PdfLine { Text = title, IsHeading = true, FontSize = 22, IsTitle = true });
            }
            
            // Extract platform name from first strong tag or paragraph after h1
            var platformMatch = System.Text.RegularExpressions.Regex.Match(html, @"<strong[^>]*>([^<]*WasteNaut[^<]*?)</strong>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (!platformMatch.Success)
            {
                platformMatch = System.Text.RegularExpressions.Regex.Match(html, @"<p[^>]*><strong[^>]*>(.*?WasteNaut.*?)</strong>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            if (platformMatch.Success)
            {
                var platform = CleanHtmlText(platformMatch.Groups[1].Value);
                if (!string.IsNullOrWhiteSpace(platform))
                    lines.Add(new PdfLine { Text = platform, IsBold = true, FontSize = 12 });
            }
            
            // Skip the first h1 (already added as title) and extract remaining h1 headings
            var h1Matches = System.Text.RegularExpressions.Regex.Matches(html, @"<h1[^>]*>(.*?)</h1>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            bool firstH1 = true;
            foreach (System.Text.RegularExpressions.Match match in h1Matches)
            {
                if (firstH1) { firstH1 = false; continue; }
                var text = CleanHtmlText(match.Groups[1].Value);
                if (!string.IsNullOrWhiteSpace(text))
                    lines.Add(new PdfLine { Text = text, IsHeading = true, FontSize = 18 });
            }
            
            // Extract table of contents section specially
            var tocMatch = System.Text.RegularExpressions.Regex.Match(html, @"<div[^>]*class=['""]toc['""][^>]*>(.*?)</div>", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (tocMatch.Success)
            {
                var tocContent = tocMatch.Groups[1].Value;
                var tocH2 = System.Text.RegularExpressions.Regex.Match(tocContent, @"<h2[^>]*>(.*?)</h2>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (tocH2.Success)
                {
                    lines.Add(new PdfLine { Text = CleanHtmlText(tocH2.Groups[1].Value), IsBold = true, FontSize = 12 });
                }
                // Extract TOC list items
                var tocLiMatches = System.Text.RegularExpressions.Regex.Matches(tocContent, @"<li[^>]*>(.*?)</li>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                foreach (System.Text.RegularExpressions.Match match in tocLiMatches)
                {
                    var text = CleanHtmlText(match.Groups[1].Value);
                    if (!string.IsNullOrWhiteSpace(text))
                        lines.Add(new PdfLine { Text = "    " + text, FontSize = 10 }); // Indented TOC items
                }
            }
            
            // Extract H2 headings and their content in document order
            // First, get the body content (excluding TOC)
            var bodyMatch = System.Text.RegularExpressions.Regex.Match(html, @"<body[^>]*>(.*?)</body>", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            string bodyHtml = bodyMatch.Success ? bodyMatch.Groups[1].Value : html;
            
            // Remove TOC section before processing
            var tocRemovedHtml = System.Text.RegularExpressions.Regex.Replace(bodyHtml, @"<div[^>]*class=['""]toc['""][^>]*>.*?</div>", "", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            // Find all H2 headings with their positions
            var h2Pattern = @"<h2[^>]*>(.*?)</h2>";
            var h2Matches = System.Text.RegularExpressions.Regex.Matches(tocRemovedHtml, h2Pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            
            // Convert to list for easier iteration
            var h2List = h2Matches.Cast<System.Text.RegularExpressions.Match>().ToList();
            
            // Process each section sequentially: H2 heading followed by its content
            for (int i = 0; i < h2List.Count; i++)
            {
                var h2Match = h2List[i];
                var h2Text = CleanHtmlText(h2Match.Groups[1].Value);
                if (string.IsNullOrWhiteSpace(h2Text) || h2Text.Contains("Table of Contents"))
                    continue;
                
                // Add the H2 heading
                lines.Add(new PdfLine { Text = h2Text, IsHeading = true, FontSize = 14, IsSectionHeading = true });
                
                // Find the section content between this H2 and the next H2 (or end of document)
                int currentH2End = h2Match.Index + h2Match.Length;
                int nextH2Start = tocRemovedHtml.Length; // Default to end if no next H2
                
                // Find next H2 position
                if (i + 1 < h2List.Count)
                {
                    nextH2Start = h2List[i + 1].Index;
                }
                
                // Extract the section content between current H2 and next H2
                int sectionLength = nextH2Start - currentH2End;
                if (sectionLength > 0 && currentH2End + sectionLength <= tocRemovedHtml.Length)
                {
                    string sectionContent = tocRemovedHtml.Substring(currentH2End, sectionLength);
                
                    // Extract list items from this section
                    var liPattern = @"<li[^>]*>(.*?)</li>";
                    var liMatches = System.Text.RegularExpressions.Regex.Matches(sectionContent, liPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                    
                    foreach (System.Text.RegularExpressions.Match liMatch in liMatches)
                    {
                        var text = CleanHtmlText(liMatch.Groups[1].Value);
                        // Remove checkbox symbols and bullets
                        text = System.Text.RegularExpressions.Regex.Replace(text, @"[□☐☑✓•]", "").Trim();
                        // Decode HTML entities
                        text = text.Replace("&deg;", " deg").Replace("&nbsp;", " ").Trim();
                        if (!string.IsNullOrWhiteSpace(text) && text.Length > 1)
                        {
                            lines.Add(new PdfLine { Text = "- " + text, FontSize = 11 });
                        }
                    }
                    
                    // Also extract paragraphs with checkboxes (for donation checklist format)
                    var pPattern = @"<p[^>]*>(.*?)</p>";
                    var pMatches = System.Text.RegularExpressions.Regex.Matches(sectionContent, pPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                    
                    foreach (System.Text.RegularExpressions.Match pMatch in pMatches)
                    {
                        var pText = CleanHtmlText(pMatch.Groups[1].Value);
                        
                        // Skip if it's a generated/footer line or platform name
                        if (pText.Contains("Generated:") || pText.Contains("tax purposes") || 
                            pText.Contains("tax advisor") || pText.Contains("For questions") || 
                            pText.Contains("contact") || pText.Contains("WasteNaut") || 
                            pText.Length < 5)
                            continue;
                        
                        // Check if it contains a checkbox (donation checklist format)
                        if (pText.Contains("□") || pText.Contains("☐") || pText.Contains("☑") || pText.Contains("✓"))
                        {
                            // Remove checkbox symbol and clean up
                            pText = System.Text.RegularExpressions.Regex.Replace(pText, @"[□☐☑✓•]", "").Trim();
                            if (pText.Length > 3)
                            {
                                lines.Add(new PdfLine { Text = "- " + pText, FontSize = 11 });
                            }
                        }
                    }
                }
            }
            
            // Extract any remaining list items that weren't under an H2 (for content before first H2 or after last H2)
            // This handles edge cases where content exists outside of H2 sections
            // Note: Paragraphs with checkboxes are now extracted within each H2 section above
            // to maintain proper document order
            
            // Extract table rows (for tax receipts and other tables)
            var tableMatch = System.Text.RegularExpressions.Regex.Match(html, @"<table[^>]*>(.*?)</table>", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (tableMatch.Success)
            {
                var tableContent = tableMatch.Groups[1].Value;
                // Extract header row
                var theadMatch = System.Text.RegularExpressions.Regex.Match(tableContent, @"<thead[^>]*>(.*?)</thead>", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (theadMatch.Success)
                {
                    var thMatches = System.Text.RegularExpressions.Regex.Matches(theadMatch.Groups[1].Value, @"<th[^>]*>(.*?)</th>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (thMatches.Count > 0)
                    {
                        var headerText = string.Join(" | ", thMatches.Cast<System.Text.RegularExpressions.Match>().Select(m => CleanHtmlText(m.Groups[1].Value)));
                        lines.Add(new PdfLine { Text = headerText, IsBold = true, FontSize = 11 });
                        lines.Add(new PdfLine { Text = new string('-', Math.Min(70, headerText.Length)), FontSize = 11 });
                    }
                }
                // Extract body rows
                var tbodyMatch = System.Text.RegularExpressions.Regex.Match(tableContent, @"<tbody[^>]*>(.*?)</tbody>", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var bodyContent = tbodyMatch.Success ? tbodyMatch.Groups[1].Value : tableContent;
                var trMatches = System.Text.RegularExpressions.Regex.Matches(bodyContent, @"<tr[^>]*>(.*?)</tr>", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                foreach (System.Text.RegularExpressions.Match trMatch in trMatches)
                {
                    var tdMatches = System.Text.RegularExpressions.Regex.Matches(trMatch.Groups[1].Value, @"<t[dh][^>]*>(.*?)</t[dh]>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (tdMatches.Count > 0)
                    {
                        var rowText = string.Join(" | ", tdMatches.Cast<System.Text.RegularExpressions.Match>().Select(m => CleanHtmlText(m.Groups[1].Value)));
                        if (!string.IsNullOrWhiteSpace(rowText) && rowText.Length > 3)
                        {
                            lines.Add(new PdfLine { Text = rowText, FontSize = 10 });
                        }
                    }
                }
            }
            
            var filteredLines = lines.Where(l => !string.IsNullOrWhiteSpace(l.Text) && !l.Text.Contains("body {") && !l.Text.Contains("font-family")).ToList();
            
            // Ensure we have at least some content - if extraction failed, try to get any text
            if (filteredLines.Count == 0)
            {
                // Try to get title at least
                var h1MatchFallback = System.Text.RegularExpressions.Regex.Match(html, @"<h1[^>]*>(.*?)</h1>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (h1MatchFallback.Success)
                {
                    var title = CleanHtmlText(h1MatchFallback.Groups[1].Value);
                    if (!string.IsNullOrWhiteSpace(title))
                        filteredLines.Add(new PdfLine { Text = title, IsTitle = true, FontSize = 22 });
                }
                
                // Fallback: extract any text from body
                var bodyMatchFallback = System.Text.RegularExpressions.Regex.Match(html, @"<body[^>]*>(.*?)</body>", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (bodyMatchFallback.Success)
                {
                    var bodyText = CleanHtmlText(bodyMatchFallback.Groups[1].Value);
                    // Extract words and create simple lines
                    var words = bodyText.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    var currentLine = "";
                    foreach (var word in words)
                    {
                        if (currentLine.Length + word.Length + 1 < 70)
                        {
                            currentLine += (currentLine.Length > 0 ? " " : "") + word;
                        }
                        else
                        {
                            if (currentLine.Length > 10 && !currentLine.Contains("Generated") && !currentLine.Contains("For questions"))
                            {
                                filteredLines.Add(new PdfLine { Text = currentLine, FontSize = 11 });
                                if (filteredLines.Count >= 10) break;
                            }
                            currentLine = word;
                        }
                    }
                    if (currentLine.Length > 10 && filteredLines.Count < 10)
                    {
                        filteredLines.Add(new PdfLine { Text = currentLine, FontSize = 11 });
                    }
                }
                
                // If still empty, add a single line to ensure PDF isn't blank
                if (filteredLines.Count == 0)
                {
                    filteredLines.Add(new PdfLine { Text = "Document content", FontSize = 12 });
                }
            }
            
            // Always ensure first line is not empty - this is critical for rendering
            if (filteredLines.Count > 0 && string.IsNullOrWhiteSpace(filteredLines[0].Text))
            {
                filteredLines[0] = new PdfLine { Text = filteredLines.Count > 1 && !string.IsNullOrWhiteSpace(filteredLines[1].Text) 
                    ? filteredLines[1].Text 
                    : "Document", FontSize = 12 };
            }
            
            return filteredLines;
        }
        
        private string CleanHtmlText(string html)
        {
            // Remove all HTML tags
            var text = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", " ");
            // Decode HTML entities
            text = text.Replace("&deg;", "°")
                      .Replace("&nbsp;", " ")
                      .Replace("&amp;", "&")
                      .Replace("&lt;", "<")
                      .Replace("&gt;", ">")
                      .Replace("&quot;", "\"")
                      .Replace("&#39;", "'")
                      .Replace("  ", " ")
                      .Trim();
            return text;
        }
        
        private byte[] GenerateTaxReceiptPdf(string donorId, string taxYear)
        {
            var random = new Random($"{donorId}{taxYear}".GetHashCode());
            var totalDonations = 5 + random.Next(0, 20);
            var totalValue = 2500 + random.Next(0, 5000);
            
            var donations = new List<(string Date, string Description, decimal Value)>();
            for (int i = 1; i <= totalDonations && i <= 10; i++)
            {
                var date = $"{taxYear}-{random.Next(1, 13):D2}-{random.Next(1, 28):D2}";
                var value = 100 + random.Next(0, 500);
                donations.Add((date, $"Food Donation #{i}", value));
            }
            
            decimal shownTotal = donations.Sum(d => d.Value);
            decimal remainingValue = totalValue - shownTotal;
            
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(50);
                    
                    page.Header()
                        .Height(80)
                        .AlignCenter()
                        .Column(column =>
                        {
                            column.Item()
                                .Text("OFFICIAL TAX RECEIPT")
                                .FontSize(24)
                                .Bold();
                            
                            column.Item()
                                .PaddingTop(5)
                                .Text("WasteNaut Food Rescue Platform")
                                .FontSize(14)
                                .Bold();
                            
                            column.Item()
                                .PaddingTop(3)
                                .Text($"Tax Year: {taxYear}")
                                .FontSize(12);
                        });
                    
                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            // Donor Info
                            column.Item()
                                .PaddingBottom(15)
                                .Column(info =>
                                {
                                    info.Item().Text($"Donor ID: {donorId}").FontSize(12).Bold();
                                    info.Item().PaddingTop(3).Text($"Donor Name: {donorId.Replace("-", " ")}").FontSize(12);
                                });
                            
                            // Summary Box
                            column.Item()
                                .PaddingVertical(10)
                                .Background("#F5F5F5")
                                .Padding(15)
                                .Border(1)
                                .BorderColor("#DDDDDD")
                                .Column(summary =>
                                {
                                    summary.Item().PaddingBottom(5).Text("Summary").FontSize(14).Bold();
                                    summary.Item().Text($"Total Donations: {totalDonations}").FontSize(12);
                                    summary.Item().PaddingTop(3).Text($"Total Donation Value: ${totalValue:F2}").FontSize(12).Bold();
                                });
                            
                            column.Item().PaddingTop(15);
                            
                            // Donation Details Table
                            column.Item()
                                .PaddingTop(10)
                                .PaddingBottom(10)
                                .Text("Donation Details")
                                .FontSize(16)
                                .Bold();
                            
                            column.Item()
                                .Table(table =>
                                {
                                    static IContainer CellStyleHeader(IContainer container)
                                    {
                                        return container
                                            .Border(1)
                                            .BorderColor("#DDDDDD")
                                            .Background("#F0F0F0")
                                            .Padding(10);
                                    }
                                    
                                    static IContainer CellStyleRow(IContainer container)
                                    {
                                        return container
                                            .Border(1)
                                            .BorderColor("#DDDDDD")
                                            .Padding(10);
                                    }
                                    
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(2);
                                    });
                                    
                                    // Header
                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyleHeader).Text("Date").Bold().FontSize(11);
                                        header.Cell().Element(CellStyleHeader).Text("Description").Bold().FontSize(11);
                                        header.Cell().Element(CellStyleHeader).AlignRight().Text("Value").Bold().FontSize(11);
                                    });
                                    
                                    // Rows
                                    foreach (var donation in donations)
                                    {
                                        table.Cell().Element(CellStyleRow).Text(donation.Date).FontSize(10);
                                        table.Cell().Element(CellStyleRow).Text(donation.Description).FontSize(10);
                                        table.Cell().Element(CellStyleRow).AlignRight().Text($"${donation.Value:F2}").FontSize(10);
                                    }
                                    
                                    // Footer row if there are more donations
                                    if (totalDonations > 10)
                                    {
                                        table.Cell().ColumnSpan(2).Element(CellStyleRow).Text($"... and {totalDonations - 10} more donations").FontSize(10).Italic();
                                        table.Cell().Element(CellStyleRow).AlignRight().Text($"${remainingValue:F2}").FontSize(10);
                                    }
                                });
                            
                            // Footer text
                            column.Item()
                                .PaddingTop(30)
                                .BorderTop(1)
                                .BorderColor("#DDDDDD")
                                .PaddingTop(15)
                                .Column(footer =>
                                {
                                    footer.Item().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC").FontSize(10).FontColor("#808080");
                                    footer.Item().PaddingTop(5).Text("This receipt is for tax purposes only.").FontSize(10).Bold();
                                    footer.Item().PaddingTop(3).Text("Consult your tax advisor for proper reporting.").FontSize(10);
                                    footer.Item().PaddingTop(3).Text("WasteNaut Food Rescue Platform | EIN: 12-3456789").FontSize(10);
                                });
                        });
                });
            });
            
            return document.GeneratePdf();
        }
        
        private class PdfLine
        {
            public string Text { get; set; } = string.Empty;
            public bool IsHeading { get; set; }
            public bool IsBold { get; set; }
            public bool IsTitle { get; set; }
            public bool IsSectionHeading { get; set; }
            public int FontSize { get; set; } = 11;
        }
        
        private byte[] CreateFormattedPdf(List<PdfLine> lines)
        {
            // Ensure we have at least one line
            if (lines.Count == 0)
            {
                lines.Add(new PdfLine { Text = "No content available", FontSize = 12 });
            }
            
            // Filter out empty lines
            lines = lines.Where(l => !string.IsNullOrWhiteSpace(l.Text)).ToList();
            
            if (lines.Count == 0)
            {
                lines.Add(new PdfLine { Text = "Document content", FontSize = 12 });
            }
            
            // Use QuestPDF to generate the PDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(50);
                    
                    page.Header()
                        .Height(60)
                        .Background("#0FF588")
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("WasteNaut Food Rescue Platform")
                        .FontSize(18)
                        .Bold()
                        .FontColor(Colors.White);
                    
                    page.Content()
                        .PaddingVertical(10)
                        .Column(column =>
                        {
                            column.Spacing(8);
                            
                            foreach (var line in lines)
                            {
                                if (string.IsNullOrWhiteSpace(line.Text))
                                    continue;
                                
                                if (line.IsTitle)
                                {
                                    column.Item()
                                        .PaddingVertical(5)
                                        .Background("#00D4AA")
                                        .Padding(10)
                                        .AlignCenter()
                                        .Text(line.Text)
                                        .FontSize(line.FontSize)
                                        .Bold()
                                        .FontColor(Colors.White);
                                }
                                else if (line.IsHeading && line.FontSize >= 18)
                                {
                                    column.Item()
                                        .PaddingVertical(5)
                                        .Background("#F2FCF8")
                                        .Padding(8)
                                        .Text(line.Text)
                                        .FontSize(line.FontSize)
                                        .Bold()
                                        .FontColor("#0AD483");
                                }
                                else if (line.IsSectionHeading || (line.IsHeading && line.FontSize == 14))
                                {
                                    column.Item()
                                        .PaddingVertical(4)
                                        .Text(line.Text)
                                        .FontSize(line.FontSize)
                                        .Bold()
                                        .FontColor("#009966");
                                    
                                    column.Item()
                                        .PaddingBottom(4)
                                        .LineHorizontal(1)
                                        .LineColor("#009966");
                                }
                                else
                                {
                                    var textBlock = column.Item()
                                        .PaddingVertical(2)
                                        .Text(line.Text)
                                        .FontSize(line.FontSize)
                                        .FontColor(Colors.Black);
                                    
                                    if (line.IsBold)
                                    {
                                        textBlock.Bold();
                                    }
                                }
                            }
                        });
                    
                    page.Footer()
                        .Height(30)
                        .Background("#D9D9D9")
                        .AlignCenter()
                        .AlignMiddle()
                        .Text(text =>
                        {
                            text.Span($"Generated: {DateTime.Now:yyyy-MM-dd} - Page ").FontSize(9).FontColor("#808080");
                            text.CurrentPageNumber().FontSize(9).FontColor("#808080");
                            text.Span(" of ").FontSize(9).FontColor("#808080");
                            text.TotalPages().FontSize(9).FontColor("#808080");
                        });
                });
            });
            
            return document.GeneratePdf();
        }

        private string GenerateFoodSafetyGuideHtml()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\">");
            sb.AppendLine("    <title>Food Safety Guidelines - WasteNaut</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; max-width: 800px; margin: 40px auto; padding: 20px; line-height: 1.8; }");
            sb.AppendLine("        h1 { color: #00ff88; border-bottom: 2px solid #00d4aa; padding-bottom: 10px; }");
            sb.AppendLine("        h2 { color: #333; margin-top: 30px; }");
            sb.AppendLine("        ul { margin: 10px 0; padding-left: 30px; }");
            sb.AppendLine("        li { margin: 8px 0; }");
            sb.AppendLine("        .toc { background: #f5f5f5; padding: 20px; margin: 20px 0; border-left: 4px solid #00ff88; }");
            sb.AppendLine("        .toc ol { margin: 10px 0; padding-left: 30px; }");
            sb.AppendLine("        @media print { body { margin: 0; } }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <h1>Food Safety Guidelines</h1>");
            sb.AppendLine("    <p><strong>WasteNaut Food Rescue Platform</strong></p>");
            sb.AppendLine();
            sb.AppendLine("    <div class='toc'>");
            sb.AppendLine("        <h2>Table of Contents:</h2>");
            sb.AppendLine("        <ol>");
            sb.AppendLine("            <li>Temperature Control</li>");
            sb.AppendLine("            <li>Storage Guidelines</li>");
            sb.AppendLine("            <li>Handling Procedures</li>");
            sb.AppendLine("            <li>Transportation</li>");
            sb.AppendLine("            <li>Distribution</li>");
            sb.AppendLine("        </ol>");
            sb.AppendLine("    </div>");
            sb.AppendLine();
            sb.AppendLine("    <h2>1. TEMPERATURE CONTROL</h2>");
            sb.AppendLine("    <ul>");
            sb.AppendLine("        <li>Perishable items must be kept at or below 40&deg;F</li>");
            sb.AppendLine("        <li>Hot foods must be kept at or above 140&deg;F</li>");
            sb.AppendLine("        <li>Use thermometers to monitor temperatures</li>");
            sb.AppendLine("        <li>Check temperatures regularly and document readings</li>");
            sb.AppendLine("        <li>Discard items that have been in the danger zone (40-140&deg;F) for more than 2 hours</li>");
            sb.AppendLine("    </ul>");
            sb.AppendLine();
            sb.AppendLine("    <h2>2. STORAGE GUIDELINES</h2>");
            sb.AppendLine("    <ul>");
            sb.AppendLine("        <li>Store items off the floor on clean shelving</li>");
            sb.AppendLine("        <li>Separate raw and cooked items</li>");
            sb.AppendLine("        <li>Label items with dates and contents</li>");
            sb.AppendLine("        <li>Follow FIFO (First In, First Out) principle</li>");
            sb.AppendLine("        <li>Maintain clean and organized storage areas</li>");
            sb.AppendLine("        <li>Store chemicals separately from food items</li>");
            sb.AppendLine("    </ul>");
            sb.AppendLine();
            sb.AppendLine("    <h2>3. HANDLING PROCEDURES</h2>");
            sb.AppendLine("    <ul>");
            sb.AppendLine("        <li>Wash hands before and after handling food</li>");
            sb.AppendLine("        <li>Use clean utensils and equipment</li>");
            sb.AppendLine("        <li>Inspect items for spoilage before acceptance</li>");
            sb.AppendLine("        <li>Reject items that show signs of spoilage</li>");
            sb.AppendLine("        <li>Wear appropriate protective gear when handling food</li>");
            sb.AppendLine("        <li>Keep work surfaces clean and sanitized</li>");
            sb.AppendLine("    </ul>");
            sb.AppendLine();
            sb.AppendLine("    <h2>4. TRANSPORTATION</h2>");
            sb.AppendLine("    <ul>");
            sb.AppendLine("        <li>Use clean, sanitized vehicles</li>");
            sb.AppendLine("        <li>Maintain proper temperatures during transport</li>");
            sb.AppendLine("        <li>Secure items to prevent damage</li>");
            sb.AppendLine("        <li>Complete transport within safe time windows</li>");
            sb.AppendLine("        <li>Use appropriate containers for different food types</li>");
            sb.AppendLine("        <li>Document transport times and conditions</li>");
            sb.AppendLine("    </ul>");
            sb.AppendLine();
            sb.AppendLine("    <h2>5. DISTRIBUTION</h2>");
            sb.AppendLine("    <ul>");
            sb.AppendLine("        <li>Distribute items promptly upon receipt</li>");
            sb.AppendLine("        <li>Maintain cold chain throughout process</li>");
            sb.AppendLine("        <li>Verify recipient capacity before delivery</li>");
            sb.AppendLine("        <li>Document all distributions</li>");
            sb.AppendLine("        <li>Provide recipients with handling instructions</li>");
            sb.AppendLine("        <li>Follow up to ensure proper storage at destination</li>");
            sb.AppendLine("    </ul>");
            sb.AppendLine();
            sb.AppendLine("    <p style='margin-top: 40px; font-size: 12px; color: #666;'>");
            sb.AppendLine($"        Generated: {DateTime.UtcNow:yyyy-MM-dd}");
            sb.AppendLine("    </p>");
            sb.AppendLine("    <p style='margin-top: 10px; font-size: 14px;'>");
            sb.AppendLine("        For questions, contact safety@wastenaut.com");
            sb.AppendLine("    </p>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            return sb.ToString();
        }

        private string GenerateDonationChecklistHtml()
        {
            return @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>Donation Best Practices Checklist - WasteNaut</title>
    <style>
        body { font-family: Arial, sans-serif; max-width: 800px; margin: 40px auto; padding: 20px; line-height: 1.8; }
        h1 { color: #00ff88; border-bottom: 2px solid #00d4aa; padding-bottom: 10px; }
        h2 { color: #333; margin-top: 30px; }
        .checkbox { font-size: 18px; margin-right: 8px; }
        @media print { body { margin: 0; } }
    </style>
</head>
<body>
    <h1>Donation Best Practices Checklist</h1>
    <p><strong>WasteNaut Food Rescue Platform</strong></p>
    
    <h2>PRE-DONATION CHECKLIST:</h2>
    <p><span class='checkbox'>□</span> Items are within expiration/use-by dates</p>
    <p><span class='checkbox'>□</span> Packaging is intact and secure</p>
    <p><span class='checkbox'>□</span> Items have been stored properly</p>
    <p><span class='checkbox'>□</span> Items show no signs of spoilage</p>
    <p><span class='checkbox'>□</span> Quantity and weight are accurately measured</p>
    <p><span class='checkbox'>□</span> Items are properly labeled</p>
    
    <h2>PACKAGING REQUIREMENTS:</h2>
    <p><span class='checkbox'>□</span> Items are in clean, food-safe containers</p>
    <p><span class='checkbox'>□</span> Containers are properly sealed</p>
    <p><span class='checkbox'>□</span> Labels include item type and quantity</p>
    <p><span class='checkbox'>□</span> Date information is clearly visible</p>
    <p><span class='checkbox'>□</span> Special handling instructions are noted</p>
    
    <h2>PREPARATION STEPS:</h2>
    <ol>
        <li>Sort items by category</li>
        <li>Check for damage or spoilage</li>
        <li>Package items securely</li>
        <li>Label clearly with contents and dates</li>
        <li>Prepare for pickup or delivery</li>
    </ol>
    
    <h2>DOCUMENTATION:</h2>
    <p><span class='checkbox'>□</span> Complete donation form</p>
    <p><span class='checkbox'>□</span> Provide accurate quantity measurements</p>
    <p><span class='checkbox'>□</span> Include any special instructions</p>
    <p><span class='checkbox'>□</span> Note pickup time preferences</p>
    <p><span class='checkbox'>□</span> Confirm contact information</p>
    
    <h2>POST-DONATION:</h2>
    <p><span class='checkbox'>□</span> Confirm receipt with recipient</p>
    <p><span class='checkbox'>□</span> Update donation records</p>
    <p><span class='checkbox'>□</span> Request feedback if available</p>
    <p><span class='checkbox'>□</span> Maintain communication with platform</p>
    
    <p style='margin-top: 40px; font-size: 12px; color: #666;'>
        Generated: " + DateTime.UtcNow.ToString("yyyy-MM-dd") + @"
    </p>
</body>
</html>";
        }

        private string GenerateTaxReceiptHtml(string donorId, string taxYear)
        {
            var random = new Random($"{donorId}{taxYear}".GetHashCode());
            var totalDonations = 5 + random.Next(0, 20);
            var totalValue = 2500 + random.Next(0, 5000);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\">");
            sb.AppendLine("    <title>Tax Receipt - WasteNaut</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: 'Times New Roman', serif; max-width: 800px; margin: 40px auto; padding: 20px; }");
            sb.AppendLine("        .header { text-align: center; border-bottom: 2px solid #000; padding-bottom: 20px; margin-bottom: 30px; }");
            sb.AppendLine("        .header h1 { margin: 0; font-size: 24px; }");
            sb.AppendLine("        .info { margin: 20px 0; }");
            sb.AppendLine("        .summary { background: #f5f5f5; padding: 20px; margin: 20px 0; border: 1px solid #ddd; }");
            sb.AppendLine("        table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
            sb.AppendLine("        th, td { border: 1px solid #ddd; padding: 10px; text-align: left; }");
            sb.AppendLine("        th { background: #f0f0f0; }");
            sb.AppendLine("        .footer { margin-top: 40px; font-size: 12px; color: #666; border-top: 1px solid #ddd; padding-top: 20px; }");
            sb.AppendLine("        @media print { body { margin: 0; } }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='header'>");
            sb.AppendLine("        <h1>OFFICIAL TAX RECEIPT</h1>");
            sb.AppendLine("        <p><strong>WasteNaut Food Rescue Platform</strong></p>");
            sb.AppendLine($"        <p>Tax Year: {taxYear}</p>");
            sb.AppendLine("    </div>");
            sb.AppendLine();
            sb.AppendLine("    <div class='info'>");
            sb.AppendLine($"        <p><strong>Donor ID:</strong> {donorId}</p>");
            sb.AppendLine($"        <p><strong>Donor Name:</strong> {donorId.Replace("-", " ")}</p>");
            sb.AppendLine("    </div>");
            sb.AppendLine();
            sb.AppendLine("    <div class='summary'>");
            sb.AppendLine("        <h2>Summary</h2>");
            sb.AppendLine($"        <p><strong>Total Donations:</strong> {totalDonations}</p>");
            sb.AppendLine($"        <p><strong>Total Donation Value:</strong> ${totalValue:F2}</p>");
            sb.AppendLine("    </div>");
            sb.AppendLine();
            sb.AppendLine("    <h2>Donation Details</h2>");
            sb.AppendLine("    <table>");
            sb.AppendLine("        <thead>");
            sb.AppendLine("            <tr><th>Date</th><th>Description</th><th>Value</th></tr>");
            sb.AppendLine("        </thead>");
            sb.AppendLine("        <tbody>");

            for (int i = 1; i <= totalDonations && i <= 10; i++)
            {
                var date = $"{taxYear}-{random.Next(1, 13):D2}-{random.Next(1, 28):D2}";
                var value = 100 + random.Next(0, 500);
                sb.AppendLine($"            <tr><td>{date}</td><td>Food Donation #{i}</td><td>${value:F2}</td></tr>");
            }

            if (totalDonations > 10)
            {
                var remainingValue = totalValue - Enumerable.Range(1, 10).Select(i => 100 + new Random($"{donorId}{taxYear}{i}".GetHashCode()).Next(0, 500)).Sum();
                sb.AppendLine($"            <tr><td colspan='2'><em>... and {totalDonations - 10} more donations</em></td><td>${remainingValue:F2}</td></tr>");
            }

            sb.AppendLine("        </tbody>");
            sb.AppendLine("    </table>");
            sb.AppendLine();
            sb.AppendLine("    <div class='footer'>");
            sb.AppendLine($"        <p>Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>");
            sb.AppendLine("        <p><strong>This receipt is for tax purposes only.</strong></p>");
            sb.AppendLine("        <p>Consult your tax advisor for proper reporting.</p>");
            sb.AppendLine("        <p>WasteNaut Food Rescue Platform | EIN: 12-3456789</p>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private string GenerateGenericDocumentHtml(string filename)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>{filename} - WasteNaut</title>
    <style>
        body {{ font-family: Arial, sans-serif; max-width: 800px; margin: 40px auto; padding: 20px; }}
        h1 {{ color: #00ff88; }}
    </style>
</head>
<body>
    <h1>Document: {filename}</h1>
    <p><strong>WasteNaut Food Rescue Platform</strong></p>
    <p>This is a placeholder document for {filename}.</p>
    <p>For more information, visit our Content Hub or contact support.</p>
    <p style='margin-top: 40px; font-size: 12px; color: #666;'>Generated: {DateTime.UtcNow:yyyy-MM-dd}</p>
</body>
</html>";
        }
    }
}

