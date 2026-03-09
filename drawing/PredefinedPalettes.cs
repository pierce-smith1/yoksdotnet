using yoksdotnet.common;

namespace yoksdotnet.drawing;

public class PredefinedPalette(string name, PaletteGroup group) : Palette, ISfEnum
{
    public static readonly PredefinedPalette Aemil = PaletteConversion.FromHexStrings(
        "Aemil",
        PaletteGroup.Fractalthorns,
        "#4aea8c",
        "#bbf3dd",
        "#2f864d",
        "#eb8213",
        "#c33535",
        "#ffffff",
        "#7e3a14"
    );

    public static readonly PredefinedPalette Loxxe = PaletteConversion.FromHexStrings(
        "Loxxe",
        PaletteGroup.Fractalthorns,
        "#2e4894",
        "#4878b4",
        "#1e2029",
        "#191d31",
        "#a8a8a8",
        "#d2cfcf",
        "#000000"
    );

    public static readonly PredefinedPalette Lotus = PaletteConversion.FromHexStrings(
        "Lotus",
        PaletteGroup.Fractalthorns,
        "#e63052",
        "#f99b8c",
        "#790f49",
        "#e1904d",
        "#7c0f44",
        "#ffffff",
        "#81163a"
    );

    public static readonly PredefinedPalette Chasnah = PaletteConversion.FromHexStrings(
        "Chasnah",
        PaletteGroup.Fractalthorns,
        "#641beb",
        "#b988ec",
        "#2b0b78",
        "#e71cf9",
        "#f318eb",
        "#ffffff",
        "#e26ce7"
    );

    public static readonly PredefinedPalette Malda = PaletteConversion.FromHexStrings(
        "Malda",
        PaletteGroup.Fractalthorns,
        "#d5c472",
        "#eae2b9",
        "#7a4c16",
        "#606060",
        "#aa6010",
        "#fbf9f1",
        "#383838"
    );

    public static readonly PredefinedPalette Evjar = PaletteConversion.FromHexStrings(
        "Evjar",
        PaletteGroup.Fractalthorns,
        "#71331b",
        "#db946a",
        "#380e1c",
        "#9073d2",
        "#f1782e",
        "#fdf1ec",
        "#3d287b"
    );

    public static readonly PredefinedPalette Romal = PaletteConversion.FromHexStrings(
        "Romal",
        PaletteGroup.Fractalthorns,
        "#68448f",
        "#cfa19a",
        "#312248",
        "#717999",
        "#6b6ce2",
        "#f0ebf5",
        "#383b4d"
    );

    public static readonly PredefinedPalette Meazs = PaletteConversion.FromHexStrings(
        "Meazs",
        PaletteGroup.Fractalthorns,
        "#de7a29",
        "#eec294",
        "#721232",
        "#db9ad8",
        "#a63fc2",
        "#fcf2ea",
        "#7e3189"
    );

    public static readonly PredefinedPalette Ellai = PaletteConversion.FromHexStrings(
        "Ellai",
        PaletteGroup.Fractalthorns,
        "#a0cd50",
        "#d5e6a8",
        "#2f6f20",
        "#7d4e29",
        "#c82222",
        "#f6faee",
        "#3e1422"
    );

    public static readonly PredefinedPalette Vette = PaletteConversion.FromHexStrings(
        "Vette",
        PaletteGroup.Fractalthorns,
        "#95c4cd",
        "#cae6e0",
        "#396578",
        "#c5b5b5",
        "#425ab3",
        "#f4f9fa",
        "#6a535b"
    );

    public static readonly PredefinedPalette Zsung = PaletteConversion.FromHexStrings(
        "Zsung",
        PaletteGroup.Fractalthorns,
        "#6a2222",
        "#d28073",
        "#35111d",
        "#362121",
        "#d71919",
        "#191313",
        "#1b1014"
    );

    public static readonly PredefinedPalette Tchesta = PaletteConversion.FromHexStrings(
        "Tchesta",
        PaletteGroup.Fractalthorns,
        "#ccced3",
        "#e6e8e9",
        "#60646f",
        "#ccced3",
        "#1cee34",
        "#474444",
        "#60646f"
    );

    public static readonly PredefinedPalette Zero = PaletteConversion.FromHexStrings(
        "Zero",
        PaletteGroup.SevenInspired,
        "#1d9ce3",
        "#0cc5e3",
        "#035ac1",
        "#f66d29",
        "#69b419",
        "#ffffff",
        "#b53410"
    );

    public static readonly PredefinedPalette Dewdrops = PaletteConversion.FromHexStrings(
        "Dewdrops",
        PaletteGroup.SevenInspired,
        "#b26666",
        "#ba92b4",
        "#8f3745",
        "#b7c393",
        "#c79057",
        "#eeedf2",
        "#6c7443"
    );

    public static readonly PredefinedPalette Succulent = PaletteConversion.FromHexStrings(
        "Succulent",
        PaletteGroup.SevenInspired,
        "#adca00",
        "#e7f023",
        "#577c00",
        "#678c01",
        "#494949",
        "#f0eed8",
        "#4e7000"
    );

    public static readonly PredefinedPalette Viride = PaletteConversion.FromHexStrings(
        "Viride",
        PaletteGroup.SevenInspired,
        "#00bb81",
        "#00c9c0",
        "#006a42",
        "#00a4a5",
        "#030502",
        "#ffffff",
        "#00444d"
    );

    public static readonly PredefinedPalette Anemone = PaletteConversion.FromHexStrings(
        "Anemone",
        PaletteGroup.SevenInspired,
        "#8a34bb",
        "#bc6bd4",
        "#392c57",
        "#87c4b5",
        "#8581cb",
        "#ffffff",
        "#3e462f"
    );

    public static readonly PredefinedPalette Crimson = PaletteConversion.FromHexStrings(
        "Crimson",
        PaletteGroup.SevenInspired,
        "#c32d74",
        "#ed4dad",
        "#5c1b2f",
        "#c20420",
        "#6d0c15",
        "#ffffff",
        "#6d0c15"
    );

    public static readonly PredefinedPalette Hazel = PaletteConversion.FromHexStrings(
        "Hazel",
        PaletteGroup.SevenInspired,
        "#fdd250",
        "#f9e8a3",
        "#ef7127",
        "#9e0601",
        "#a51e0a",
        "#ffffff",
        "#4b090b"
    );

    public static readonly PredefinedPalette Canyon = PaletteConversion.FromHexStrings(
        "Canyon",
        PaletteGroup.SevenInspired,
        "#7a3226",
        "#ef6b37",
        "#0c0608",
        "#cd7535",
        "#516fab",
        "#fbe7d7",
        "#b9390a"
    );

    public static readonly PredefinedPalette Glacier = PaletteConversion.FromHexStrings(
        "Glacier",
        PaletteGroup.SevenInspired,
        "#bec2e7",
        "#fbfbef",
        "#476088",
        "#f8b1ab",
        "#132b73",
        "#ffffff",
        "#99879f"
    );

    public static readonly PredefinedPalette Sunset = PaletteConversion.FromHexStrings(
        "Sunset",
        PaletteGroup.SevenInspired,
        "#d79fcc",
        "#fac496",
        "#4f4f83",
        "#df5f18",
        "#d16928",
        "#ffffff",
        "#99542b"
    );

    public static readonly PredefinedPalette Overgrowth = PaletteConversion.FromHexStrings(
        "Overgrowth",
        PaletteGroup.SevenInspired,
        "#acc04d",
        "#e2e864",
        "#354516",
        "#cf7da1",
        "#2e442d",
        "#f6f6f6",
        "#be417b"
    );

    public static readonly PredefinedPalette Mesa = PaletteConversion.FromHexStrings(
        "Mesa",
        PaletteGroup.SevenInspired,
        "#c36b11",
        "#d7b689",
        "#7c1b0b",
        "#907087",
        "#ba4e1d",
        "#ffffff",
        "#765570"
    );

    public static readonly PredefinedPalette Lavender = PaletteConversion.FromHexStrings(
        "Lavender",
        PaletteGroup.SevenInspired,
        "#ec94c4",
        "#ffc4eb",
        "#63435b",
        "#376842",
        "#2d493b",
        "#f0feff",
        "#384f53"
    );

    public static readonly PredefinedPalette Chroma = PaletteConversion.FromHexStrings(
        "Chroma",
        PaletteGroup.SevenInspired,
        "#9d0246",
        "#ce5801",
        "#1b1b25",
        "#ffca72",
        "#494949",
        "#f1f2f4",
        "#003077"
    );

    public static readonly PredefinedPalette Bronze = PaletteConversion.FromHexStrings(
        "Bronze",
        PaletteGroup.SevenInspired,
        "#a8773a",
        "#c1b58e",
        "#421801",
        "#8c4121",
        "#703905",
        "#cfbea2",
        "#2c1808"
    );

    public static readonly PredefinedPalette Facade = PaletteConversion.FromHexStrings(
        "Facade",
        PaletteGroup.SevenInspired,
        "#c9ceec",
        "#fdfeff",
        "#798fc1",
        "#77abf6",
        "#264e82",
        "#ffffff",
        "#4188e0"
    );

    public static readonly PredefinedPalette Railway = PaletteConversion.FromHexStrings(
        "Railway",
        PaletteGroup.SevenInspired,
        "#007cc6",
        "#7db0df",
        "#012353",
        "#ece89e",
        "#a09b40",
        "#ebe8ee",
        "#8c701b"
    );

    public static readonly PredefinedPalette Knight = PaletteConversion.FromHexStrings(
        "Knight",
        PaletteGroup.SevenInspired,
        "#3d678d",
        "#5d88b2",
        "#193759",
        "#d0d1d3",
        "#1e2e62",
        "#ffffff",
        "#354350"
    );

    public static readonly PredefinedPalette Reflector = PaletteConversion.FromHexStrings(
        "Reflector",
        PaletteGroup.SevenInspired,
        "#0d3384",
        "#4a6fa3",
        "#000018",
        "#74370b",
        "#fbfbfb",
        "#eac165",
        "#4f1a00"
    );

    public static readonly PredefinedPalette Watercolor = PaletteConversion.FromHexStrings(
        "Watercolor",
        PaletteGroup.SevenInspired,
        "#e5e7dc",
        "#ffffff",
        "#8b94a9",
        "#e02128",
        "#30acce",
        "#ffffff",
        "#e67f23"
    );

    public static readonly PredefinedPalette Distortion = PaletteConversion.FromHexStrings(
        "Distortion",
        PaletteGroup.SevenInspired,
        "#671248",
        "#381228",
        "#d71a8f",
        "#01b0c3",
        "#671248",
        "#f9deef",
        "#007f9f"
    );

    public static readonly PredefinedPalette Spores = PaletteConversion.FromHexStrings(
        "Spores",
        PaletteGroup.SevenInspired,
        "#faae24",
        "#fbc332",
        "#b56615",
        "#9dc622",
        "#000a12",
        "#ffffff",
        "#2da75e"
    );

    public static readonly PredefinedPalette DarkHeart = PaletteConversion.FromHexStrings(
        "Dark Heart",
        PaletteGroup.SevenInspired,
        "#ce1570",
        "#ec157d",
        "#4a1833",
        "#23ad79",
        "#f37123",
        "#212330",
        "#29585a"
    );

    public static readonly PredefinedPalette Vines = PaletteConversion.FromHexStrings(
        "Vines",
        PaletteGroup.SevenInspired,
        "#92b43c",
        "#d5e85c",
        "#073f0e",
        "#fbdfba",
        "#7880c8",
        "#f0f0f0",
        "#bbb0c0"
    );

    public static readonly PredefinedPalette Sugar = PaletteConversion.FromHexStrings(
        "Sugar",
        PaletteGroup.SevenInspired,
        "#92e5d3",
        "#9be3d5",
        "#2daba6",
        "#dd79b9",
        "#d974b6",
        "#ffffff",
        "#e49d29"
    );

    public static readonly PredefinedPalette Ranger = PaletteConversion.FromHexStrings(
        "Ranger",
        PaletteGroup.SevenInspired,
        "#9d5024",
        "#c05d1f",
        "#292421",
        "#da8f65",
        "#878105",
        "#ffffff",
        "#9d727c"
    );

    public static readonly PredefinedPalette Harbor = PaletteConversion.FromHexStrings(
        "Harbor",
        PaletteGroup.SevenInspired,
        "#ab87b9",
        "#bba6cb",
        "#6657a9",
        "#f47b18",
        "#dc4600",
        "#ffffff",
        "#9e4c10"
    );

    public static readonly PredefinedPalette Duskrock = PaletteConversion.FromHexStrings(
        "Duskrock",
        PaletteGroup.SevenInspired,
        "#d75b11",
        "#fac751",
        "#681800",
        "#203167",
        "#fae6df",
        "#0c0c18",
        "#151845"
    );

    public static readonly PredefinedPalette Longhorn = PaletteConversion.FromHexStrings(
        "Longhorn",
        PaletteGroup.SevenInspired,
        "#456890",
        "#657ea7",
        "#132a6d",
        "#132c6e",
        "#ecab75",
        "#ffffff",
        "#010a31"
    );

    public static readonly PredefinedPalette Snowcapped = PaletteConversion.FromHexStrings(
        "Snowcapped",
        PaletteGroup.SevenInspired,
        "#ffa561",
        "#ffe2aa",
        "#7c2221",
        "#fffbfc",
        "#c582bc",
        "#ffffff",
        "#91afed"
    );

    public static readonly PredefinedPalette Glimmer = PaletteConversion.FromHexStrings(
        "Glimmer",
        PaletteGroup.SevenInspired,
        "#5e3bc9",
        "#aea0ef",
        "#0d1270",
        "#266f64",
        "#d39192",
        "#ffe692",
        "#143e30"
    );

    public static readonly PredefinedPalette Crescent = PaletteConversion.FromHexStrings(
        "Crescent",
        PaletteGroup.XpInspired,
        "#256ac7",
        "#8892d0",
        "#02499b",
        "#9099e8",
        "#0b3e77",
        "#fbfffd",
        "#4d5ab4"
    );

    public static readonly PredefinedPalette Fall = PaletteConversion.FromHexStrings(
        "Fall",
        PaletteGroup.XpInspired,
        "#c46918",
        "#dea50d",
        "#794315",
        "#3a3531",
        "#727d39",
        "#f6f4f4",
        "#481b18"
    );

    public static readonly PredefinedPalette Aitutaki = PaletteConversion.FromHexStrings(
        "Aitutaki",
        PaletteGroup.XpInspired,
        "#1ab8e5",
        "#8edbef",
        "#114871",
        "#b9cbdf",
        "#753746",
        "#ffffff",
        "#88a2bd"
    );

    public static readonly PredefinedPalette Sonoma = PaletteConversion.FromHexStrings(
        "Sonoma",
        PaletteGroup.XpInspired,
        "#6e9812",
        "#9cc336",
        "#344813",
        "#6293f0",
        "#2461ed",
        "#f9f3f7",
        "#2565eb"
    );

    public static readonly PredefinedPalette Refraction = PaletteConversion.FromHexStrings(
        "Refraction",
        PaletteGroup.XpInspired,
        "#258eea",
        "#195ba2",
        "#59d7d3",
        "#452eae",
        "#5144d8",
        "#d8ebf7",
        "#4756f7"
    );

    public static readonly PredefinedPalette Swarm = PaletteConversion.FromHexStrings(
        "Swarm",
        PaletteGroup.XpInspired,
        "#eb662d",
        "#f2b72b",
        "#a34835",
        "#14a8ff",
        "#050a06",
        "#f4f5b3",
        "#0c6bc7"
    );

    public static readonly PredefinedPalette Hunter = PaletteConversion.FromHexStrings(
        "Hunter",
        PaletteGroup.XpInspired,
        "#9e9181",
        "#c8bbab",
        "#3c362a",
        "#373026",
        "#261f19",
        "#ffffff",
        "#27201a"
    );

    public static readonly PredefinedPalette Stucco = PaletteConversion.FromHexStrings(
        "Stucco",
        PaletteGroup.XpInspired,
        "#af6981",
        "#e6bbe9",
        "#663c46",
        "#5d63ed",
        "#c01317",
        "#ffffff",
        "#242e6b"
    );

    public static readonly PredefinedPalette Starflower = PaletteConversion.FromHexStrings(
        "Starflower",
        PaletteGroup.XpInspired,
        "#eb115b",
        "#f438b3",
        "#960627",
        "#ffffff",
        "#f95ed3",
        "#fccf02",
        "#fae8b8"
    );

    public static readonly PredefinedPalette Perfection = PaletteConversion.FromHexStrings(
        "Perfection",
        PaletteGroup.XpInspired,
        "#dffeff",
        "#f3feff",
        "#acdcea",
        "#a5e5e5",
        "#ddffff",
        "#acdcea",
        "#86d0cf"
    );

    public static readonly PredefinedPalette Energy = PaletteConversion.FromHexStrings(
        "Energy",
        PaletteGroup.XpInspired,
        "#ff7ef2",
        "#f2f2f2",
        "#e009e0",
        "#fffcc2",
        "#ffe9ff",
        "#ff01d5",
        "#fda4c2"
    );

    public static readonly PredefinedPalette Cyclamen = PaletteConversion.FromHexStrings(
        "Cyclamen",
        PaletteGroup.XpInspired,
        "#935390",
        "#b082c2",
        "#67264e",
        "#026e6e",
        "#1f857a",
        "#ffffff",
        "#042b26"
    );

    public static readonly PredefinedPalette Prism = PaletteConversion.FromHexStrings(
        "Prism",
        PaletteGroup.XpInspired,
        "#d7d7d7",
        "#b1b1b1",
        "#616161",
        "#ff6c55",
        "#28db8a",
        "#ffffff",
        "#2f6ba4"
    );

    public static readonly PredefinedPalette Kalahari = PaletteConversion.FromHexStrings(
        "Kalahari",
        PaletteGroup.XpInspired,
        "#ce3d10",
        "#f76534",
        "#6c1f0d",
        "#a89dad",
        "#4374d0",
        "#f0f0f3",
        "#696b92"
    );

    public static readonly PredefinedPalette Wavelet = PaletteConversion.FromHexStrings(
        "Wavelet",
        PaletteGroup.XpInspired,
        "#009df6",
        "#00c8f6",
        "#005ef4",
        "#b53c03",
        "#01d3f4",
        "#ffffff",
        "#793c1f"
    );

    public static readonly PredefinedPalette Moss = PaletteConversion.FromHexStrings(
        "Moss",
        PaletteGroup.XpInspired,
        "#787566",
        "#d3c9ca",
        "#26281b",
        "#35502d",
        "#5473c3",
        "#e5e7f1",
        "#192e0f"
    );

    public static readonly PredefinedPalette Sunflower = PaletteConversion.FromHexStrings(
        "Sunflower",
        PaletteGroup.XpInspired,
        "#e6b30a",
        "#fff3e3",
        "#8e5707",
        "#f0eff4",
        "#72904a",
        "#ffffff",
        "#7890e6"
    );

    public static readonly PredefinedPalette Maelstrom = PaletteConversion.FromHexStrings(
        "Maelstrom",
        PaletteGroup.XpInspired,
        "#128dff",
        "#0abeff",
        "#06278e",
        "#0abeff",
        "#00c5f9",
        "#0a171f",
        "#113bdd"
    );

    public static readonly PredefinedPalette Desert = PaletteConversion.FromHexStrings(
        "Desert",
        PaletteGroup.XpInspired,
        "#fd903d",
        "#ffa652",
        "#7d5639",
        "#f8bf7a",
        "#405ea7",
        "#fcf3ea",
        "#fb9a65"
    );

    public static readonly PredefinedPalette Professional = PaletteConversion.FromHexStrings(
        "Professional",
        PaletteGroup.XpInspired,
        "#34548d",
        "#a1bff5",
        "#0b111f",
        "#fb6100",
        "#eb4c36",
        "#edf0f5",
        "#ab2a02"
    );
    public string Name => name;
    public PaletteGroup Group => group;

    public override string ToString() => name;
}

