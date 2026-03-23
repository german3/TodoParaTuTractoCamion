const ExcelJS = require('exceljs');
const fs = require('fs');
const path = require('path');

async function extractImages() {
    const xlsxPath = 'D:\\Codrava\\Proyecto\\Lista Productos.xlsx';
    const outputDir = 'D:\\Codrava\\Proyecto\\TodoParaTuTractoCamion.BlazorWasm\\wwwroot\\images\\productos';

    const workbook = new ExcelJS.Workbook();
    await workbook.xlsx.readFile(xlsxPath);
    const worksheet = workbook.worksheets[0];

    // THE EXACT NAMES THE USER PROVIDED, but we will clean them
    const targets = [
        "(PZ) ZEPELIN COMPLETO KW 22 LED AMBAR",
        "(PZA) LIMPIAPARABRISAS HELLA CLEANTECH 21\"",
        "(PZA) FARO INTERNATIONAL 4300 L",
        "(PAR) ALAMBRON PORTA LODERA SENCILLOS M",
        "TTFCU00004 / IPS TTFCU00004 (JGO)",
        "(PAR) TAPETE INTERNATIONAL PROSTAR"
    ];

    const clean = (s) => s.replace(/\s+/g, ' ').trim().toUpperCase();

    const results = [];

    worksheet.eachRow((row, rowNumber) => {
        let rowText = "";
        row.eachCell((cell) => {
            rowText += (cell.value || "").toString() + " ";
        });
        const cleanRowText = clean(rowText);

        targets.forEach(target => {
            if (cleanRowText.includes(clean(target))) {
                console.log(`MATCH Row ${rowNumber} for ${target}`);
                
                worksheet.getImages().forEach((image) => {
                    const imgRow = image.range.tl.nativeRow + 1;
                    if (Math.abs(imgRow - rowNumber) <= 1) { // Flex range
                        const imgData = workbook.model.media[image.imageId];
                        const fileName = `img_final_r${rowNumber}_${image.imageId}.${imgData.extension}`;
                        fs.writeFileSync(path.join(outputDir, fileName), imgData.buffer);
                        console.log(`  -> Saved ${fileName}`);
                        results.push({ target, fileName, rowNumber });
                    }
                });
            }
        });
    });
}

extractImages().catch(err => console.error(err));
