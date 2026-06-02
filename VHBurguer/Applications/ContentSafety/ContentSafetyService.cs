
using Google.GenAI;

namespace VHBurguer.Applications.ContentSafety
{
    public class ContentSafetyService : IContentSafetyRepository
    {
        // Chave da API do Gemini:
        private readonly string _apiKey;

        public ContentSafetyService(IConfiguration configuration)
        {
            // Validações:
            // Verificar se existe na variável de ambiente.
            _apiKey = configuration["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY") ??
                throw new Exception("API Key não configurada");
        }

        public async Task<(bool aprovado, string msg)> ValidarConteudo(string texto)
        {
            // Gemini --> texto --> válido??? --> true/false
            if(string.IsNullOrEmpty(_apiKey))
            {
                return (false, "API Key não configurada");
            }

            try
            {
                // Cliente responsável pela comunicação com o Gemini.
                var client = new Client(apiKey: _apiKey);

                // Definir o prompt a ser passado ao Gemini.
                string prompt = $@"Você é um moderador de conteúdo extremamente rigoroso para uma plataforma pública.

                    Analise o TEXTO abaixo considerando as regras:

                    - NÃO é permitido:
                      - palavrões, xingamentos ou linguagem vulgar (ex: ""caralho"", ""porra"", ""merda"", etc.)
                      - conteúdo ofensivo, agressivo ou desrespeitoso
                      - conteúdo com duplo sentido ou conotação sexual
                      - qualquer linguagem inadequada para ambiente profissional ou educacional
                      - conteúdo ilegal (drogas, armas, etc.)

                    - Mesmo que esteja em tom informal ou ""brincadeira"", ainda deve ser considerado INSEGURO.

                    - Seja extremamente conservador: na dúvida, classifique como INSEGURO.

                    Responda APENAS com:

                    SEGURO ou INSEGURO: [breve motivo em português]

                    TEXTO:{texto}";

                // Envia o texto para analise da IA.
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash-lite", contents: prompt);

                // Obter a resposta gerada pela IA.
                string result = response.Text?.Trim() ?? "";

                if(result.StartsWith("INSEGURO"))
                {
                    return(false, result);
                }

                return (true, "Textos seguros! ;)");

            }
            catch(Exception ex)
            {
                return (false, "Erro na IA." + ex.Message);
            }
        }
    }
}
