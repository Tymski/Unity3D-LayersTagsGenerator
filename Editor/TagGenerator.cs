using System.CodeDom;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TNRD.CodeGeneration.Tags
{
    public class TagGenerator : ICodeGenerator
    {
        [MenuItem("Tools/Code Generation/Tags")]
        private static void Execute()
        {
            TagGenerator generator = new();
            generator.Generate();
        }

        public void Generate()
        {
            string[] tags = InternalEditorUtility.tags
                .OrderBy(x => x)
                .ToArray();

            CodeCompileUnit compileUnit = new();
            CodeNamespace codeNamespace = new();
            CodeTypeDeclaration classDeclaration = new("Tags")
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
            };

            foreach (string tag in tags)
            {
                CodeMemberField field = new()
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Const,
                    Name = Utilities.GetScreamName(tag),
                    Type = new CodeTypeReference(typeof(string)),
                    InitExpression = new CodePrimitiveExpression(tag)
                };
                classDeclaration.Members.Add(field);
            }

            codeNamespace.Types.Add(classDeclaration);
            compileUnit.Namespaces.Add(codeNamespace);

            Utilities.GenerateToFile(compileUnit, Application.dataPath + "/Generated", "Tags.cs");
        }
    }
}
