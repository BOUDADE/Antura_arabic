﻿using UnityEngine;
using UnityEngine.UI;
using EA4S.Helpers;
using TMPro;

namespace EA4S.UI
{
    public class TextRenderUtility : MonoBehaviour
    {
        TMP_Text m_TextComponent;
        TMP_TextInfo textInfo;

        public int yOffset = 10;

        public void ShowInfo()
        {
            m_TextComponent = gameObject.GetComponent<TMP_Text>();
            textInfo = m_TextComponent.textInfo;

            int characterCount = textInfo.characterCount;

            if (characterCount > 1) {
                for (int i = 0; i < characterCount; i++) {
                    //Debug.Log("CAHR " + characterCount + ": " + TMPro.TMP_TextUtilities.StringToInt(textInfo.characterInfo[characterCount].character.ToString()));
                    Debug.Log("CHAR: " + i
                              + "index: " + textInfo.characterInfo[i].index
                              + "char: " + textInfo.characterInfo[i].character.ToString()
                              + "UNICODE: " + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[i].character)
                             );
                }
                //textInfo.characterInfo[1].textElement.yOffset += yOffset;
            }
        }

        public void AdjustDiacriticPositions()
        {
            m_TextComponent = gameObject.GetComponent<TMP_Text>();
            m_TextComponent.ForceMeshUpdate();
            textInfo = m_TextComponent.textInfo;

            int characterCount = textInfo.characterCount;

            if (characterCount > 1) {
                int newYOffset = 0;
                int charPosition = 1;

                if (ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[0].character) == "0627"
                    && ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[1].character) == "064B") {
                    newYOffset = yOffset;
                }

                if (ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[0].character) == "0623"
                    && ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[1].character) == "064E") {
                    newYOffset = yOffset;
                }

                if (newYOffset != 0) {
                    // method 1
                    //TMP_TextElement updateTextElement = textInfo.characterInfo[charPosition].textElement;
                    //updateTextElement.yOffset += newYOffset;
                    //textInfo.characterInfo[charPosition].textElement = updateTextElement;

                    // method 2
                    //textInfo.characterInfo[charPosition].ascender += newYOffset;

                    // method 3
                    // Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.
                    TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[charPosition];
                    // Get the index of the material used by the current character.
                    int materialIndex = textInfo.characterInfo[charPosition].materialReferenceIndex;
                    // Get the index of the first vertex used by this text element.
                    int vertexIndex = textInfo.characterInfo[charPosition].vertexIndex;

                    // Get the cached vertices of the mesh used by this text element (character or sprite).
                    Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

                    // Determine the center point of each character at the baseline.
                    //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
                    // Determine the center point of each character.
                    Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                    // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
                    // This is needed so the matrix TRS is applied at the origin for each character.
                    Vector3 offset = charMidBasline;

                    Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                    destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                    destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                    destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                    destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                    textInfo.meshInfo[charPosition].mesh.vertices = textInfo.meshInfo[charPosition].vertices;
                    m_TextComponent.UpdateGeometry(textInfo.meshInfo[charPosition].mesh, charPosition);
                    //m_TextComponent.UpdateVertexData();

                    Debug.Log("DIACRITIC: diacritic pos fixed for " + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[1].character) + " by " + newYOffset);
                }


                for (int i = 0; i < characterCount; i++) {
                    //Debug.Log("CAHR " + characterCount + ": " + TMPro.TMP_TextUtilities.StringToInt(textInfo.characterInfo[characterCount].character.ToString()));
                    Debug.Log("DIACRITIC: " + i
                              //+ "index: " + textInfo.characterInfo[i].index
                              + " char: " + textInfo.characterInfo[i].character.ToString()
                              + " UNICODE: " + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[i].character)
                             );
                }
                //textInfo.characterInfo[1].textElement.yOffset += yOffset;
            }


        }
    }
}