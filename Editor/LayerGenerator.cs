using System.CodeDom;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TNRD.CodeGeneration.Layers
{
    public class LayerGenerator : ICodeGenerator
    {
        [MenuItem("Tools/Code Generation/Layers")]
        private static void Execute()
        {
            var generator = new LayerGenerator();
            generator.Generate();
        }

        public void Generate()
        {
            string[] layers = InternalEditorUtility.layers
                .OrderBy(x => x)
                .ToArray();

            var codeCompileUnit = new CodeCompileUnit();
            var codeNamespace = new CodeNamespace();
            var codeTypeDeclaration = new CodeTypeDeclaration("Layers")
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
            };

            for (int i = 0; i < layers.Length; i++)
            {
                string layer = layers[i];
                string layerName = Utilities.GetScreamName(layer);
                string maskName = layerName + "_MASK";
                int layerValue = LayerMask.NameToLayer(layer);

                CodeMemberField layerField = new CodeMemberField(typeof(int), layerName)
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Const,
                    InitExpression = new CodePrimitiveExpression(layerValue)
                };

                CodeMemberField maskField = new CodeMemberField(typeof(int), maskName)
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Const,
                    InitExpression = new CodePrimitiveExpression(1 << layerValue)
                };

                codeTypeDeclaration.Members.Add(layerField);
                codeTypeDeclaration.Members.Add(maskField);
            }

            codeNamespace.Types.Add(codeTypeDeclaration);
            codeCompileUnit.Namespaces.Add(codeNamespace);

            Utilities.GenerateToFile(codeCompileUnit, Application.dataPath + "/Generated", "Layers.cs");
        }
    }
}
