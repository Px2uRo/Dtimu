namespace WebDriveDtimu.Models

open System.IO

module Scanner=

    
    let mutable VedioPaths : string list = []

    let rec private scanDir (dir: string) =
        seq {
        // 当前目录 mp4
        for file in Directory.EnumerateFiles(dir, "*.mp4") do
            yield file

        // 遍历子目录
        for sub in Directory.EnumerateDirectories(dir) do
            yield! scanDir sub
    }

    /// 异步开始扫描
    let scanBegin dir =
        async {
            VedioPaths <- scanDir dir |> Seq.toList
    }

    let scanOne dir =
        scanDir dir |> Seq.toList
