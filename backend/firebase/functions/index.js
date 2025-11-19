import { onRequest } from "firebase-functions/v2/https";
import express from "express";
import OpenAI from "openai";
import dotenv from "dotenv";
import npcData from "./npcData.js";

// dotenv.config();
const app = express();

app.get('/dialogue/:npcName', async (req, res) => {


    const currentNPC = npcData[req.params.npcName];
    const systemPrompt = currentNPC.systemPrompt;
    // const mission_prompt = current_npc.mission_prompts[req.params.mission_name];
    const missionPrompt = currentNPC.missionPrompts["m_1_d_1"];
    
    
    const messages = [
        {
            "role": "system",
            "content": systemPrompt
        },
        {
            "role": "developer",
            "content": missionPrompt
        },
        {

            "role": "user",
            "content": "Hello"
        }
    ]


    const openai = new OpenAI({
        baseURL: "https://api.openai.com/v1",
        apiKey: process.env.LIVE_OPENAIKEY
    });

    try {
        const response = await openai.responses.create({
        model: "gpt-5-nano",
        previous_response_id: "resp_0a273c472fd42cf100691bf98159f8819789dde44f5589558f",
        input: messages,
        store: true
        });

    res.send(response.output_text + "<br>" + response.id);

    
    } catch (error) {
        console.error(error);
        res.status(500).send("Error calling LLM");
    }

    
});

app.get('/evaluate', async (req, res) => {


    const semanticEval = async (prevRespID, refPhrase, userInput) => {

        const openai = new OpenAI({
            apiKey: process.env.LIVE_OPENAIKEY,
            baseURL: "https://api.openai.com/v1"
        });
        
        try {
            const response = await openai.responses.create({
                model: "gpt-5-nano",
                previous_response_id: prevRespID,
                input: [
                    {
                        role: "system",
                        content: `
                        You are a dialogue evaluator for player inputs that are in response to NPC outputs.
                        You can see the recent conversation between the NPC and the player (user).
                        Use that context plus the extra information in this message to judge the latest player reply.
                        Do not rewrite and do not correct the user input or user responses.
                        You must be strict in regard to grammar and spelling errors.
                        You will be given the conversaton history, a correct reference phrase (Correct Reference Response), and a response from the user (User Input). 
                        `,
                    },
                    {
                        role: "user",
                        content: `
        
                        Correct Reference Response: "${refPhrase}"
                        User Input: "${userInput}"
        
                        Is the user input phrase free of spelling errors?
                        If so, is the user input free of language syntax errors?
                        If so, does the user input phrase make sense as a reply to the LLM NPCs last output and is similar to the correct reference response?
                        Return a strict JSON object with: {"passed": "yes"|"no", "reason": "<short explanation>"}
                        
                        `,
                    },
                ],
                store: true,
            });
        
            return response.output_text

        } catch (error) {
            console.error(error);
            res.status(500).send("Error calling LLM");
        }
    }
    
});

export const callLLM = onRequest(app);
