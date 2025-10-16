import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

export async function exportElementToPdf(el: HTMLElement, filename: string){
  const canvas = await html2canvas(el);
  const imgData = canvas.toDataURL('image/png');
  const pdf = new jsPDF({ orientation: 'p', unit: 'pt', format: 'a4' });
  const pageWidth = pdf.internal.pageSize.getWidth();
  const ratio = pageWidth / canvas.width;
  const imgHeight = canvas.height * ratio;
  pdf.addImage(imgData, 'PNG', 0, 20, pageWidth, imgHeight);
  const now = new Date();
  pdf.setFontSize(10);
  pdf.text(`Generado: ${now.toLocaleString()}`, 20, 12);
  pdf.save(filename);
}
