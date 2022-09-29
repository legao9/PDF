﻿using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class RightToLeftExamples
    {
        [Test]
        public void Unconstrained()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .AlignCenter()
                        .AlignMiddle()
                        .ContentFromRightToLeft()
                        .Unconstrained()
                        .Background(Colors.Red.Medium)
                        .Height(200)
                        .Width(200);
                });
        }
        
        [Test]
        public void Row()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .ContentFromRightToLeft()
                        .Border(1)
                        .Row(row =>
                        {
                            row.ConstantItem(200).Background(Colors.Red.Lighten2).Height(200);
                            row.ConstantItem(150).Background(Colors.Green.Lighten2).Height(200);
                            row.ConstantItem(100).Background(Colors.Blue.Lighten2).Height(200);
                        });
                });
        }
        
        [Test]
        public void MinimalBox()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .ContentFromRightToLeft()
                        .Border(1)
                        .MinimalBox()
                        .Column(column =>
                        {
                            column.Item().Background(Colors.Red.Lighten2).Width(200).Height(200);
                            column.Item().Background(Colors.Green.Lighten2).Width(150).Height(150);
                            column.Item().Background(Colors.Blue.Lighten2).Width(100).Height(100);
                        });
                });
        }
        
        [Test]
        public void Table()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .ContentFromRightToLeft()
                        .Border(1)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(200);
                                columns.ConstantColumn(150);
                                columns.ConstantColumn(100);
                            });

                            table.Cell().Background(Colors.Red.Lighten2).Height(200);
                            table.Cell().Background(Colors.Green.Lighten2).Height(200);
                            table.Cell().Background(Colors.Blue.Lighten2).Height(200);
                        });
                });
        }
    }
}