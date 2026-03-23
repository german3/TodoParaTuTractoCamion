import openpyxl
from openpyxl_image_loader import SheetImageLoader
import os
import uuid

def extract_images():
    xlsx_path = r"D:\Codrava\Proyecto\Lista Productos.xlsx"
    output_dir = r"D:\Codrava\Proyecto\TodoParaTuTractoCamion.BlazorWasm\wwwroot\images\productos"
    
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)

    print(f"Loading workbook: {xlsx_path}")
    wb = openpyxl.load_workbook(xlsx_path)
    sheet = wb.active # Use the first active sheet
    
    # We use a special loader to handle embedded images
    image_loader = SheetImageLoader(sheet)
    
    target_rows = [1, 44, 91, 146, 277, 284, 385]
    
    for row in target_rows:
        print(f"Processing row {row}...")
        product_name = sheet.cell(row=row, column=1).value
        print(f"  Product: {product_name}")
        
        # Searching for images in standard columns (I=9, J=10, K=11)
        # However, openpyxl_image_loader works by cell address (e.g. 'I1')
        cols = ['I', 'J', 'K']
        for col in cols:
            cell_address = f"{col}{row}"
            try:
                if image_loader.image_in(cell_address):
                    image = image_loader.get(cell_address)
                    file_id = str(uuid.uuid4())[:8]
                    file_name = f"img_extracted_r{row}_{col}_{file_id}.png"
                    img_path = os.path.join(output_dir, file_name)
                    image.save(img_path)
                    print(f"    -> Saved {col}{row} to {file_name}")
            except Exception as e:
                # Some pictures might be floating or not exactly in the cell
                # We can't easily find floating pictures with this simple loader
                pass

    print("Extraction complete.")

if __name__ == "__main__":
    # Ensure dependencies are available (openpyxl-image-loader might need pip install)
    extract_images()
