import { onRequest } from "firebase-functions/v2/https";
import express from "express";
import cors from "cors";
import OpenAI from "openai";
import npcData from "./npcData.js";

const app = express();


app.use(cors({
  origin: [
    "https://cs467-live.github.io",
  ],
  credentials: true,
  methods: ["GET", "POST"],
  allowedHeaders: ["Content-Type", "Authorization"],
}));

app.use(express.json());

const openai = new OpenAI({
  baseURL: "https://api.openai.com/v1",
  apiKey: process.env.LIVE_OPENAIKEY || "",
});

const languageCodeMap = {
  en: "English",
  es: "Spanish",
  ar: "Arabic",
  fr: "French",
  he: "Hebrew",
  de: "German",
  ja: "Japanese",
  ko: "Korean",
  zh: "Chinese",
};

const callDialogueLLM = async (npcID, dialogueIndex = 0, userInput = "", prevRespID = undefined, languageName = "English") => {
  const currentNPC = npcData[npcID];

  if (!currentNPC) {
    return {
      outputText: "INVALID_NPC",
      responseID: undefined,
    };
  }

  const systemPrompt = currentNPC.systemPrompt;
  const dialogueName = currentNPC.dialogueSequence[dialogueIndex];
  const currentDialogue = currentNPC.missionPrompts[dialogueName];

  const messages = [
    {
      "role": "system",
      "content": `${systemPrompt} The language of the NPC is ${languageName}.`,
    },
    {
      "role": "developer",
      "content": currentDialogue,
    },
    {
      "role": "user",
      "content": userInput,
    },
  ];

  const response = await openai.responses.create({
    model: "gpt-5-nano",
    ...(prevRespID && { previous_response_id: prevRespID }),
    input: messages,
    store: true,
  });

  return {
    outputText: response.output_text,
    responseID: response.id,
  };
};


app.post("/dialogue", async (req, res) => {
  try {
    const {npcID, dialogueIndex, userInput, prevRespID, language} = req.body;

    const languageName = languageCodeMap[language];
    if (!languageName) {
      res.status(400).send("Invalid language");
      return;
    }

    const dialogueResponse = await callDialogueLLM(npcID, dialogueIndex, userInput, prevRespID, languageName);

    if (dialogueResponse.responseID === undefined) {
      res.status(400).send("Invalid NPC ID");
      return;
    }

    res.send(dialogueResponse);
  } catch (error) {
    console.error(error);
    res.status(500).send("Error calling LLM");
  }
});


const semanticEval = async (npcID, dialogueIndex = 0, userInput = "", prevRespID = undefined, languageName = "English") => {
  const currentNPC = npcData[npcID];

  if (!currentNPC) {
    return {
      passed: "no",
      reason: "INVALID_NPC",
    };
  }

  const dialogueName = currentNPC.dialogueSequence[dialogueIndex];
  const correctResponse = currentNPC.correctResponses[dialogueName];

  const messages = [
    {
      role: "system",
      content: `
            You are a dialogue evaluator for player inputs that are in response to NPC outputs.
            You can see the recent conversation between the NPC and the player (user).
            Use that context plus the extra information in this message to judge the latest player reply.
            Do not rewrite and do not correct the user input or user responses.
            You must be strict in regard to grammar and spelling errors.
            You will be given the conversaton history, a correct reference phrase (Correct Reference Response), and a response from the user (User Input). 
            The language of the NPC is ${languageName}.
            `,
    },
    {
      role: "user",
      content: `

            Correct Reference Response: "${correctResponse}"
            User Input: "${userInput}"

            Assuming that we were to translate the Correct Reference Response into ${languageName}.
            Is the user input the same language as ${languageName}?
            Is the user input phrase free of spelling errors?
            If so, is the user input free of language syntax errors?
            If so, does the user input phrase make sense as a reply to the LLM NPCs last output and is similar to the correct reference response?
            Return a strict JSON object with: {"passed": "yes"|"no", "reason": "<short explanation>"}
            
            `,
    },
  ];

  const response = await openai.responses.create({
    model: "gpt-5-nano",
    ...(prevRespID && { previous_response_id: prevRespID }),
    input: messages,
    store: true,
  });

  const outputText = response.output_text.trim();

  try {
    const parsedOutput = JSON.parse(outputText);
    return parsedOutput;
  } catch (error) {
    console.log("JSON parse failed, extracting text...");

    const extractedText = outputText.toLowerCase().substring(0, 5);

    let passed = "no";

    if (extractedText.includes("yes")) {
      passed = "yes";
    }

    return {
      passed: passed,
      reason: outputText,
    };
  }
};

app.post("/evaluate", async (req, res) => {
  try {
    const {npcID, dialogueIndex, userInput, prevRespID, language} = req.body;

    const languageName = languageCodeMap[language];
    if (!languageName) {
      res.status(400).send("Invalid language");
      return;
    }

    const evalResponse = await semanticEval(npcID, dialogueIndex, userInput, prevRespID, languageName);

    if (evalResponse.reason === "INVALID_NPC") {
      res.status(400).send("Invalid NPC ID");
      return;
    }

    res.send(evalResponse);
  } catch (error) {
    console.error(error);
    res.status(500).send("Error calling LLM");
  }
});

export const callLLM = onRequest(app);
