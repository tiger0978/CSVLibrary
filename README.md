# CSVLibrary 🚀

CSVLibrary 是一個針對高效能讀寫所設計的 C# CSV 處理工具庫。
本專案除了提供基礎的 CSV 檔案轉換與讀寫功能外，主要側重於**探討與實作 C# 效能優化技巧**，並透過 `BenchmarkDotNet` 進行精準的效能基準測試。

## ✨ 核心優化技術 (Performance Optimizations)

本專案實作了以下三種進階的 C# 效能優化手段，以克服處理大型 CSV 檔案時常見的 CPU 與 GC (垃圾回收) 瓶頸：

### 1. 運算式樹快取 (Expression Tree Delegates)
傳統的 ORM 或 CSV 映射工具多半依賴 `System.Reflection` 來動態取得或設定物件屬性，這會造成極大的效能耗損。
* 本專案在 `OriginalWriterVSOptimizedWriter` 與 `SpliteVsSpanSlice` 中，利用 `Expression.Lambda` 動態編譯出強型別的 `GetterDelegate` 與 `SetterDelegate`。
* 透過快取這些 Delegate，後續的屬性讀寫效能幾乎與硬編碼 (Hardcode) 直接存取無異，大幅超越 `PropertyInfo.GetValue()` 與 `SetValue()`。

### 2. Span<T> 與記憶體切片 (Zero-Allocation Parsing)
在解析 CSV 逐行資料時，避免使用會產生大量垃圾物件的 `String.Split(',')`。
* 專案內建的 `ReadCSVBySpan<T>` 利用 `ReadOnlySpan<char>` 在原始字串記憶體上滑動。
* 透過 `.Slice()` 與 `.IndexOf(',')` 尋找欄位邊界，避免了實體化中介字串陣列 (String Array) 所帶來的記憶體配置與 GC 壓力。

### 3. StringBuilder 容量預配置 (Capacity Pre-allocation)
* 針對大量寫入情境，預先宣告固定大小的 `StringBuilder` (例如 `Capacity = 90`) 與字元陣列緩衝區 (`char[] buffer`)。
* 有效避免了字串串接過程中，底層陣列因容量不足而頻繁觸發的擴容與記憶體複製開銷。

## 🛠️ 專案結構

* **CSVHelper.cs**: 核心操作類別，包含傳統反射讀寫以及優化過後的 `ReadCSVBySpan<T>` 方法。
* **OriginalWriterVSOptimizedWriter.cs**: 寫入效能測試 (Reflection vs. Expression Tree vs. Buffer)。
* **SpliteVsSpanSlice.cs**: 讀取與字串切割效能測試 (`String.Split` vs. `ReadOnlySpan.Slice`)。
* **Program.cs**: 專案進入點，負責啟動 `BenchmarkDotNet` 進行效能跑分。

## 🚀 快速起步

### 寫入 CSV 檔案
```csharp
CSVHelper helper = new CSVHelper(@"C:\Your\Path");
var data = new CSVModel()
{
    dateTime = "2023-07-26",
    cost = "222",
    item = "飲食",
    account = "現金",
    member = "自己"
};

// 將單一物件寫入 CSV
helper.WriteToCSV<CSVModel>("data.csv", data);
