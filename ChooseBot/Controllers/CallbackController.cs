using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VkNet.Abstractions;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using ChooseBot.Configs;
using ChooseBot.Models;
using VkNet.Model.Keyboard;
using VkNet.Enums.SafetyEnums;

namespace ChooseBot.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IVkApi _vkApi;
        private StateService _stateService;

        public CallbackController(IConfiguration configuration, IVkApi vkApi, StateService stateService)
        {
            _configuration = configuration;
            _vkApi = vkApi;
            _stateService = stateService;
        }

        [HttpPost]
        public IActionResult Callback([FromBody] Updates updates)
        {
            // Проверяем, что находится в поле "type" 
            switch (updates.Type)
            {
                // Если это уведомление для подтверждения адреса
                case "confirmation":
                    // Отправляем строку для подтверждения 
                    return Ok(_configuration["Config:Confirmation"]);
                case "message_new":
                    {
                        var msg = Message.FromJson(new VkResponse(updates.Object));

                        if (msg.Body == "Начать")
                        {
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                UserId = msg.UserId,
                                RandomId = new DateTime().Millisecond,
                                PeerId = msg.PeerId,
                                Message = StringConfigs.HelloString,
                                Keyboard = new VkNet.Model.Keyboard.MessageKeyboard
                                {
                                    OneTime = false,
                                    Buttons = new List<List<MessageKeyboardButton>>()
                                    {
                                        new List<MessageKeyboardButton>
                                        {
                                            new MessageKeyboardButton
                                            {
                                                Action = new MessageKeyboardButtonAction
                                                {
                                                    Payload = "{\"button\": \"1\"}",
                                                    Label = "Правила",
                                                    Type = KeyboardButtonActionType.Text,
                                                },
                                                Color = KeyboardButtonColor.Default
                                            },
                                            new MessageKeyboardButton
                                            {
                                                Action = new MessageKeyboardButtonAction
                                                {
                                                    Payload = "{\"button\": \"2\"}",
                                                    Label = "Начать игру",
                                                    Type = KeyboardButtonActionType.Text,
                                                },
                                                Color = KeyboardButtonColor.Positive
                                            }
                                        }
                                    }
                                }
                            });
                            if (_stateService.States.Keys.ToReadOnlyCollection().Contains(msg.UserId.Value))
                                _stateService.States[msg.UserId.Value] = State.None;
                            else
                                _stateService.States.Add(msg.UserId.Value, State.None);
                            break;
                        }

                        if (msg.Body == "Правила")
                        {
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                UserId = msg.UserId,
                                RandomId = new DateTime().Millisecond,
                                PeerId = msg.PeerId,
                                Message = StringConfigs.Rules,
                                Keyboard = new VkNet.Model.Keyboard.MessageKeyboard
                                {
                                    OneTime = false,
                                    Buttons = new List<List<MessageKeyboardButton>>()
                                    {
                                        new List<MessageKeyboardButton>
                                        {
                                            new MessageKeyboardButton
                                            {
                                                Action = new MessageKeyboardButtonAction
                                                {
                                                    Payload = "{\"button\": \"2\"}",
                                                    Label = "Начать игру",
                                                    Type = KeyboardButtonActionType.Text,
                                                },
                                                Color = KeyboardButtonColor.Positive
                                            }
                                        }
                                    }
                                }
                            });
                            _stateService.States[msg.UserId.Value] = State.Rules;
                            break;
                        }

                        if (msg.Body == "Начать игру")
                        {
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                UserId = msg.UserId,
                                RandomId = new DateTime().Millisecond,
                                PeerId = msg.PeerId,
                                Message = "Введите количество участников от 10 до 20",
                            });
                            _stateService.States[msg.UserId.Value] = State.StartGame;
                            break;
                        }

                        if (_stateService.States[msg.UserId.Value] == State.StartGame)
                        {
                            int count = 0;
                            try
                            {
                                count = Convert.ToInt32(msg.Body);
                                if (count < 10 || count > 20)
                                    throw new Exception();
                            }
                            catch
                            {
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    UserId = msg.UserId,
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = msg.PeerId,
                                    Message = "Введите количество участников от 10 до 20",
                                });
                                break;
                            }
                            _stateService.States[msg.UserId.Value] = State.Game;
                            List<int> usedIds = new List<int>();
                            Random r = new Random();
                            for (int i = 0; i < count; i++)
                            {
                                int id;
                                do
                                {
                                    id = r.Next(0, Persons.PersonList.Count - 1);
                                }
                                while (usedIds.Contains(id));
                                usedIds.Add(id);
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    UserId = msg.UserId,
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = msg.PeerId,
                                    Message = $"Игрок №{i+1} \n{Persons.PersonList[id].ToString()}",
                                });
                            }
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                UserId = msg.UserId,
                                RandomId = new DateTime().Millisecond,
                                PeerId = msg.PeerId,
                                Message = $"Условия мира \n {Situations.SituationsList[r.Next(0, Situations.SituationsList.Count)]}",
                                Keyboard = new VkNet.Model.Keyboard.MessageKeyboard
                                {
                                    OneTime = false,
                                    Buttons = new List<List<MessageKeyboardButton>>()
                                    {
                                        new List<MessageKeyboardButton>
                                        {
                                            new MessageKeyboardButton
                                            {
                                                Action = new MessageKeyboardButtonAction
                                                {
                                                    Payload = "{\"button\": \"1\"}",
                                                    Label = "Закончить игру",
                                                    Type = KeyboardButtonActionType.Text,
                                                },
                                                Color = KeyboardButtonColor.Default
                                            },
                                            new MessageKeyboardButton
                                            {
                                                Action = new MessageKeyboardButtonAction
                                                {
                                                    Payload = "{\"button\": \"2\"}",
                                                    Label = "Выслать дополнительную характеристику",
                                                    Type = KeyboardButtonActionType.Text,
                                                },
                                                Color = KeyboardButtonColor.Positive
                                            }
                                        }
                                    }
                                }
                            });
                            _stateService.States[msg.UserId.Value] = State.Additional;
                        }

                        if (msg.Body == "Выслать дополнительную характеристику")
                        {
                            Random r = new Random(); 
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                UserId = msg.UserId,
                                RandomId = new DateTime().Millisecond,
                                PeerId = msg.PeerId,
                                Message = AdditionalCharacteristics.AdditionalList[r.Next(0, AdditionalCharacteristics.AdditionalList.Count - 1)],
                                Keyboard = new VkNet.Model.Keyboard.MessageKeyboard
                                {
                                    OneTime = false,
                                    Buttons = new List<List<MessageKeyboardButton>>()
                                    {
                                        new List<MessageKeyboardButton>
                                        {
                                            new MessageKeyboardButton
                                            {
                                                Action = new MessageKeyboardButtonAction
                                                {
                                                    Payload = "{\"button\": \"1\"}",
                                                    Label = "Закончить игру",
                                                    Type = KeyboardButtonActionType.Text,
                                                },
                                                Color = KeyboardButtonColor.Default
                                            },
                                            new MessageKeyboardButton
                                            {
                                                Action = new MessageKeyboardButtonAction
                                                {
                                                    Payload = "{\"button\": \"2\"}",
                                                    Label = "Выслать дополнительную характеристику",
                                                    Type = KeyboardButtonActionType.Text,
                                                },
                                                Color = KeyboardButtonColor.Positive
                                            }
                                        }
                                    }
                                }
                            });
                        }

                        if (msg.Body == "Закончить игру")
                        {
                            _stateService.States[msg.UserId.Value] = State.None;
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                UserId = msg.UserId,
                                RandomId = new DateTime().Millisecond,
                                PeerId = msg.PeerId,
                                Message = "Спасибо за игру!",
                                Keyboard = new VkNet.Model.Keyboard.MessageKeyboard
                                {
                                    OneTime = false,
                                    Buttons = new List<List<MessageKeyboardButton>>()
                                    {
                                        new List<MessageKeyboardButton>
                                        {
                                            new MessageKeyboardButton
                                            {
                                                Action = new MessageKeyboardButtonAction
                                                {
                                                    Payload = "{\"button\": \"1\"}",
                                                    Label = "Начать",
                                                    Type = KeyboardButtonActionType.Text,
                                                },
                                                Color = KeyboardButtonColor.Default
                                            }
                                        }
                                    }
                                }
                            });
                        }
                        /*_vkApi.Messages.Send(new MessagesSendParams
                        {
                            UserId = msg.UserId,
                            RandomId = new DateTime().Millisecond,
                            PeerId = msg.PeerId,
                            Message = msg.Body,
                        });*/
                        break;
                    }
            }
            // Возвращаем "ok" серверу Callback API
            return Ok("ok");
        }
    }
}