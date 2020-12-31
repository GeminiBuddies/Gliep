namespace GeminiLab.Glos {
    public static class GlosOpExecutionInfo {
        // How many loops are required to execute this op
        public static int[] Phases { get; } = {
            // 0x00
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 2, 2, 3, 3, 2, 2,
            // 0x10
            1, 1, 4, 4, 4, 4, 4, 0,
            1, 1, 0, 0, 1, 1, 1, 1,
            // 0x20
            1, 1, 1, 1, 0, 0, 0, 0,
            1, 1, 0, 0, 1, 1, 0, 0,
            // 0x30
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 0, 0,
            // 0x40
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            // 0x50
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 0,
            // 0x60
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            // 0x70
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            // 0x80
            1, 1, 1, 1, 1, 0, 0, 0,
            1, 1, 1, 1, 1, 0, 0, 0,
            // 0x90
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            // 0xa0
            1, 1, 0, 0, 0, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            // 0xb0
            1, 1, 1, 1, 1, 1, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            // 0xc0
            1, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            // 0xd0
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            // 0xe0
            1, 1, 1, 1, 1, 1, 1, 1,
            0, 0, 0, 0, 0, 0, 0, 0,
            // 0xf0
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
        };
    }
}
