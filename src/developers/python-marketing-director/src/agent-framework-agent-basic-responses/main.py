# Copyright (c) Microsoft. All rights reserved.

import os

from agent_framework import Agent
from agent_framework.foundry import FoundryChatClient
from agent_framework_foundry_hosting import ResponsesHostServer
from azure.identity import DefaultAzureCredential
from dotenv import load_dotenv

# Load environment variables from .env file
load_dotenv()


def main():
    model_name = os.getenv("AZURE_AI_MODEL_DEPLOYMENT_NAME") or os.getenv("FOUNDRY_MODEL_NAME")
    if not model_name:
        raise RuntimeError(
            "Model deployment name is not configured. Set "
            "AZURE_AI_MODEL_DEPLOYMENT_NAME or FOUNDRY_MODEL_NAME."
        )

    client = FoundryChatClient(
        project_endpoint=os.environ["FOUNDRY_PROJECT_ENDPOINT"],
        model=model_name,
        credential=DefaultAzureCredential(),
    )

    agent = Agent(
        client=client,
        instructions="""<papel>
                    Atue como um estrategista de conteúdo para Youtube que cria planejamento de 90 dias capazes de acumular autoridade ao longo do tempo
                    </papel>
                    <tarefa>
                    Monte um calendario de conteúdo de 90 dias equilibrando crescimento por SEO e potencial viral para um canal novo
                    </tarefa>
                    <passos>
                    1.Pergunte qual é o nicho antes de começar
                    2.Gere 12 titulos otimizados para busca
                    3. Defina uma frequência de postagem sustentável.
                    4. Classifique cada vídeo como prioridade SEO ou proridade Viral
                    5. Mapeie a progressão dos conteúdos mostrando como cada vídeo constrói autoridade
                    </passos>
                    <regras>
                    - Os titulos SEO devem conter palavras-chave que as pessoas realmente pesquisam.
                    - Os vídeos virais precisam ter um conceito forte de gancho, não apenas um título chamativo.
                    - A progressáo deve mostrar claramente a construção de autoridade.
                    </regras>
                    <saída>
                    12 títulos -> frequência de publicação -> Seo vs Viral -> Progressão de Autoridade
                    </saída>""",
        # History will be managed by the hosting infrastructure, thus there
        # is no need to store history by the service. Learn more at:
        # https://developers.openai.com/api/reference/resources/responses/methods/create
        default_options={"store": False},
    )

    server = ResponsesHostServer(agent)
    server.run()


if __name__ == "__main__":
    main()
