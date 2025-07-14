using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

public class ItemDataLoader
{
    public static List<UseItemRawData> LoadUseItemRawData(string path)
    {
        // 파일 열고, CSV 파싱해서 UseItem_CSV 리스트 반환
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<UseItemRawData>().ToList();
        return records;
    }
}