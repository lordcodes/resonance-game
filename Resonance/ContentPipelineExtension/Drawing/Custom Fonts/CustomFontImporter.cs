using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace ContentPipelineExtension
{

    [ContentProcessor(DisplayName = "CustomFontProcessor")]
    class CustomFontProcessor : ContentProcessor<string[], CustomFont>
    {
        public override CustomFont Process(string[] input, ContentProcessorContext context)
        {
            CustomFont font = new CustomFont();
            foreach (string inputFilename in input)
            {
                string letterName = Path.GetFileNameWithoutExtension(inputFilename);
                ExternalReference<TextureContent> textureReference = new ExternalReference<TextureContent>(inputFilename);
                TextureContent texture = context.BuildAndLoadAsset<TextureContent, TextureContent>(textureReference, "TextureProcessor");
                BitmapContent image = texture.Faces[0][0];
                font.add(letterName[0],texture);
            }
            return font;
        }
    }

    [ContentTypeWriter]
    class CustomFontContentWriter : ContentTypeWriter<CustomFont>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Resonance.CustomFontContentReader, Resonance";
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "Resonance.ImportedCustomFont, Resonance";
        }

        protected override void Write(ContentWriter output, CustomFont value)
        {
            Dictionary<char, TextureContent> dictionary = value.Dictionary;
            output.Write(dictionary.Count);
            foreach (KeyValuePair<char, TextureContent> pair in dictionary)
            {
                output.Write(pair.Key);
                output.WriteObject<TextureContent>(pair.Value);
            }
        }
    }
}