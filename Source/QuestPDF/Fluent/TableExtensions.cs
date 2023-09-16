﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Elements.Table;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class TableColumnsDefinitionDescriptor
    {
        internal List<TableColumnDefinition> Columns { get; } = new();
        
        /// <summary>
        /// Defines a column of constant size that occupies the specified horizontal space.
        /// </summary>
        /// <returns>The container of the newly created column.</returns>
        public void ConstantColumn(float width, Unit unit = Unit.Point)
        {
            ComplexColumn(constantWidth: width.ToPoints(unit));
        }
        
        /// <summary>
        /// Defines a column with a relative size that adjusts its width in relation to other relative columns.
        /// <a href="https://www.questpdf.com/api-reference/table.html#column-definitions">Learn more</a>
        /// </summary>
        /// <example>
        /// For a table 100 points wide with three columns: a relative size of 1, a relative size of 5, and a constant size of 10 points, they will span 15 points, 75 points, and 10 points respectively.
        /// </example>
        /// <returns>The container for the newly defined column.</returns>
        public void RelativeColumn(float width = 1)
        {
            ComplexColumn(relativeWidth: width);
        }
        
        private void ComplexColumn(float constantWidth = 0, float relativeWidth = 0)
        {
            var columnDefinition = new TableColumnDefinition(constantWidth, relativeWidth);
            Columns.Add(columnDefinition);
        }
    }

    public class TableCellDescriptor
    {
        private ICollection<TableCell> Cells { get; }

        internal TableCellDescriptor(ICollection<TableCell> cells)
        {
            Cells = cells;
        }
        
        /// <summary>
        /// Inserts a new item into the table element.
        /// </summary>
        /// <returns>The container for the newly inserted cell. Provides options to adjust the cell's position, size, and content.</returns>
        public ITableCellContainer Cell()
        {
            var cell = new TableCell();
            Cells.Add(cell);
            return cell;
        }
    }
    
    public class TableDescriptor
    {
        internal List<TableColumnDefinition> Columns { get; private set; }

        private Table HeaderTable { get; } = new();
        private Table ContentTable { get; } = new();
        private Table FooterTable { get; } = new();

        /// <summary>
        /// Specifies the order and size of the table columns.
        /// <a href="https://www.questpdf.com/api-reference/table.html#column-definitions">Learn more</a>
        /// </summary>
        /// <remarks>
        /// This configuration affects both the main content as well as the header and footer sections.
        /// </remarks>
        /// <param name="handler">Handler to define columns of the table.</param>
        public void ColumnsDefinition(Action<TableColumnsDefinitionDescriptor> handler)
        {
            var descriptor = new TableColumnsDefinitionDescriptor();
            handler(descriptor);

            HeaderTable.Columns = descriptor.Columns;
            ContentTable.Columns = descriptor.Columns;
            FooterTable.Columns = descriptor.Columns;
        }
        
        /// <summary>
        /// Adjusts rendering algorithm to better handle complex table structures, especially those spanning multiple pages. 
        /// This applies a unique rule to the final cells in each column, ensuring they stretch to fill the table's bottom edge.
        /// Such an approach can enhance your table's visual appeal.
        /// <a href="https://www.questpdf.com/api-reference/table.html#extend-last-cells-to-table-bottom">Learn more</a>
        /// </summary>
        public void ExtendLastCellsToTableBottom()
        {
            ContentTable.ExtendLastCellsToTableBottom = true;
        }
        
        /// <summary>
        /// Specifies a table header that appears on each page, positioned above the main content.
        /// The cell placement and dimensions in this header are distinct from those in the main content.
        /// <a href="https://www.questpdf.com/api-reference/table.html#table-header-footer">Learn more</a>
        /// </summary>
        /// <param name="handler">Handler for configuring the header cells.</param>
        public void Header(Action<TableCellDescriptor> handler)
        {
            var descriptor = new TableCellDescriptor(HeaderTable.Cells);
            handler(descriptor);
        }
        
        /// <summary>
        /// Specifies a table footer that appears on each page, positioned below the main content.
        /// The placement and dimensions of cells within this footer are distinct from the main content.
        /// <a href="https://www.questpdf.com/api-reference/table.html#table-header-footer">Learn more</a>
        /// </summary>
        public void Footer(Action<TableCellDescriptor> handler)
        {
            var descriptor = new TableCellDescriptor(FooterTable.Cells);
            handler(descriptor);
        }
        
        /// <summary>
        /// Inserts a new item into the table element.
        /// </summary>
        /// <returns>The container for the newly inserted cell, enabling customization of its position, size, and content.</returns>
        public ITableCellContainer Cell()
        {
            var cell = new TableCell();
            ContentTable.Cells.Add(cell);
            return cell;
        }

        internal IElement CreateElement()
        {
            var container = new Container();

            ConfigureTable(HeaderTable);
            ConfigureTable(ContentTable);
            ConfigureTable(FooterTable);
            
            container
                .Decoration(decoration =>
                {
                    decoration.Before().Element(HeaderTable);
                    decoration.Content().Element(ContentTable);
                    decoration.After().Element(FooterTable);
                });

            return container;
        }

        private static void ConfigureTable(Table table)
        {
            if (!table.Columns.Any())
                throw new DocumentComposeException($"Table should have at least one column. Please call the '{nameof(ColumnsDefinition)}' method to define columns.");
            
            table.PlanCellPositions();
            table.ValidateCellPositions();
        }
    }
    
    public static class TableExtensions
    {
        /// <summary>
        /// <para>Renders a set of items utilizing the table layout algorithm.</para>
        /// <para>
        /// Items may be auto-placed based on the order they're called or can have assigned specific column and row positions. Cells can also span multiple columns and/or rows.
        /// </para>
        /// </summary>
        /// <param name="handler">Handler to define the table content.</param>
        public static void Table(this IContainer element, Action<TableDescriptor> handler)
        {
            var descriptor = new TableDescriptor();
            handler(descriptor);
            element.Element(descriptor.CreateElement());
        }
    }

    public static class TableCellExtensions
    {
        private static ITableCellContainer TableCell(this ITableCellContainer element, Action<TableCell> handler)
        {
            if (element is TableCell tableCell)
                handler(tableCell);
            
            return element;
        }
        
        /// <summary>
        /// Specifies the column position (horizontal axis) of the cell.
        /// <a href="https://www.questpdf.com/api-reference/table.html#basic-usage">Learn more</a>
        /// </summary>
        /// <param name="value">Columns are numbered starting with 1.</param>
        public static ITableCellContainer Column(this ITableCellContainer tableCellContainer, uint value)
        {
            return tableCellContainer.TableCell(x => x.Column = (int)value);
        }
        
        /// <summary>
        /// Defines the number of columns a cell spans in the horizontal axis.
        /// <a href="https://www.questpdf.com/api-reference/table.html#row-spans-and-column-spans">Learn more</a>
        /// </summary>
        /// <remarks>
        /// Useful when creating complex layouts.
        /// </remarks>
        public static ITableCellContainer ColumnSpan(this ITableCellContainer tableCellContainer, uint value)
        {
            return tableCellContainer.TableCell(x => x.ColumnSpan = (int)value);
        }
        
        /// <summary>
        /// Specifies the row position (vertical axis) of the cell.
        /// <a href="https://www.questpdf.com/api-reference/table.html#basic-usage">Learn more</a>
        /// </summary>
        /// <param name="value">Rows are numbered starting with 1.</param>
        public static ITableCellContainer Row(this ITableCellContainer tableCellContainer, uint value)
        {
            return tableCellContainer.TableCell(x => x.Row = (int)value);
        }
        
        /// <summary>
        /// Defines the number of rows a cell spans in the vertical axis.
        /// <a href="https://www.questpdf.com/api-reference/table.html#row-spans-and-column-spans">Learn more</a>
        /// </summary>
        /// <remarks>
        /// Useful when creating complex layouts.
        /// </remarks>
        public static ITableCellContainer RowSpan(this ITableCellContainer tableCellContainer, uint value)
        {
            return tableCellContainer.TableCell(x => x.RowSpan = (int)value);
        }
    }
}