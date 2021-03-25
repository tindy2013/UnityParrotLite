using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MU3.Client;
using AMDaemon.Allnet;
using MU3.User;
using MU3.Util;
using MU3.Operation;
using System.Reflection;
using System.Diagnostics;
using Harmony;
using MU3.AM;

namespace UnityParrotLite.Components
{
    class PacketPatches
    {
        public static void Patch()
        {
            Harmony.PatchAllInType(typeof(PacketPatches));
        }

		public class Buffer
		{
			private StringBuilder stringBuilder_ = new StringBuilder();

			private List<object> array_ = new List<object>();

			public int Count => array_.Count;

			public void clear()
			{
				stringBuilder_.Length = 0;
				array_.Clear();
			}

			public void add(object obj, bool isNew)
			{
				array_.Add(obj);
				stringBuilder_.Append((!isNew) ? '0' : '1');
			}

			public string toString()
			{
				return stringBuilder_.ToString();
			}

			public T[] toArray<T>() where T : class
			{
				T[] array = new T[array_.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array_[i] as T;
				}
				return array;
			}
		}

		public static void UpdateUserDetail(ref UserManager instance, ref UserAll userAll)
		{
			if (!instance.IsGuest)
			{
				UserData userData = new UserData();
				instance.userDetail.updatePlayDate();
				instance.userDetail.copyTo(userData);
				userAll.userData = new UserData[1] { userData };
				MU3.Client.UserOption userOption = new MU3.Client.UserOption();
				userOption.copyFrom(instance.userOption);
				userAll.userOption = new MU3.Client.UserOption[1] { userOption };
			}
		}

		public static void UpdatePlaylogList(ref UserManager instance, ref UserAll userAll, ref bool flag)
		{
			userAll.userPlaylogList = instance.userLocal.playLogs.ToArray();
			flag |= instance.userLocal.playLogs.Count > 0;
		}

		public static void UpdateSessionlogList(ref UserManager instance, ref UserAll userAll, ref bool isLogout)
		{
			if (isLogout)
			{
				DateTime now = DateTime.Now;
				UserSessionLog userSessionLog = new UserSessionLog();
				userSessionLog.sortNumber = NetPacketUtil.toSortNumber(instance.userLocal.loginDateTime);
				userSessionLog.placeId = Auth.LocationId;
				userSessionLog.playDate = NetPacketUtil.toLogDateString(now);
				userSessionLog.userPlayDate = NetPacketUtil.toString(now);
				userSessionLog.isPaid = instance.userLocal.isPaid;
				userAll.userSessionlogList = new UserSessionLog[1] { userSessionLog };
			}
			else
			{
				userAll.userSessionlogList = new UserSessionLog[0];
			}
		}

		public static void UpdateUserActivityList(ref UserManager instance, ref UserAll userAll)
		{
			List<MU3.Client.UserActivity> userActivityList = new List<MU3.Client.UserActivity>();
			foreach (var activity in instance.userActivityPlay)
			{
				MU3.Client.UserActivity userActivity = new MU3.Client.UserActivity();
				activity.copyTo(userActivity);
				userActivityList.Add(userActivity);
			}
			foreach (var activity in instance.userActivityMusic)
			{
				MU3.Client.UserActivity userActivity = new MU3.Client.UserActivity();
				activity.copyTo(userActivity);
				userActivityList.Add(userActivity);
			}
			userAll.userActivityList = userActivityList.ToArray();
		}
		public static void UpdateUserRecentRating(ref UserManager instance, ref UserAll userAll)
		{
			List<MU3.Client.UserRecentRating> userRecentRatings = new List<MU3.Client.UserRecentRating>();
			foreach (var rating in instance.userRecentRating)
			{
				MU3.Client.UserRecentRating userRecentRating = new MU3.Client.UserRecentRating();
				rating.copyTo(userRecentRating);
				userRecentRatings.Add(userRecentRating);
			}
			userAll.userRecentRatingList = userRecentRatings.ToArray();
		}
		public static void UpdateUserBpBase(ref UserManager instance, ref UserAll userAll)
		{
			List<UserBpBase> userBpBases = new List<UserBpBase>();
			foreach (var bp in instance.userRecentBattleScore)
			{
				UserBpBase userBpBase = new UserBpBase()
				{
					musicId = bp.musicId,
					difficultId = NetPacketUtil.toServerFumenDifficulty(bp.difficultId),
					romVersionCode = bp.romVersionCode,
					score = bp.score
				};
				userBpBases.Add(userBpBase);
			}
			userAll.userBpBaseList = userBpBases.ToArray();
		}
		public static void UpdateBestNew(ref UserManager instance, ref UserAll userAll)
		{
			List<MU3.Client.UserRatingBaseBestNew> userRatingBaseBestNews = new List<MU3.Client.UserRatingBaseBestNew>();
			foreach (var bestnew in instance.userRatingBaseBestNew)
			{
				MU3.Client.UserRatingBaseBestNew userRatingBaseBestNew = new MU3.Client.UserRatingBaseBestNew();
				bestnew.copyTo(userRatingBaseBestNew);
				userRatingBaseBestNews.Add(userRatingBaseBestNew);
			}
			userAll.userRatingBaseBestNewList = userRatingBaseBestNews.ToArray();
		}
		public static void UpdateBest(ref UserManager instance, ref UserAll userAll)
		{
			List<MU3.Client.UserRatingBaseBest> userRatingBaseBests = new List<MU3.Client.UserRatingBaseBest>();
			foreach (var best in instance.userRatingBaseBest)
			{
				MU3.Client.UserRatingBaseBest userRatingBaseBest = new MU3.Client.UserRatingBaseBest();
				best.copyTo(userRatingBaseBest);
				userRatingBaseBests.Add(userRatingBaseBest);
			}
			userAll.userRatingBaseBestList = userRatingBaseBests.ToArray();
		}
		public static void UpdateHot(ref UserManager instance, ref UserAll userAll)
		{
			List<MU3.Client.UserRatingBaseHot> userRatingBaseHots = new List<MU3.Client.UserRatingBaseHot>();
			foreach (var hot in instance.userRatingBaseHot)
			{
				MU3.Client.UserRatingBaseHot userRatingBaseHot = new MU3.Client.UserRatingBaseHot();
				hot.copyTo(userRatingBaseHot);
				userRatingBaseHots.Add(userRatingBaseHot);
			}
			userAll.userRatingBaseHotList = userRatingBaseHots.ToArray();
		}
		public static void UpdateNext(ref UserManager instance, ref UserAll userAll)
		{
			List<MU3.Client.UserRatingBaseNext> userRatingBaseNexts = new List<MU3.Client.UserRatingBaseNext>();
			foreach (var next in instance.userRatingBaseNext)
			{
				MU3.Client.UserRatingBaseNext userRatingBaseNext = new MU3.Client.UserRatingBaseNext();
				next.copyTo(userRatingBaseNext);
				userRatingBaseNexts.Add(userRatingBaseNext);
			}
			userAll.userRatingBaseNextList = userRatingBaseNexts.ToArray();
		}
		public static void UpdateNextNew(ref UserManager instance, ref UserAll userAll)
		{
			List<MU3.Client.UserRatingBaseNextNew> userRatingBaseNextNews = new List<MU3.Client.UserRatingBaseNextNew>();
			foreach (var nextnew in instance.userRatingBaseNextNew)
			{
				MU3.Client.UserRatingBaseNextNew userRatingBaseNextNew = new MU3.Client.UserRatingBaseNextNew();
				nextnew.copyTo(userRatingBaseNextNew);
				userRatingBaseNextNews.Add(userRatingBaseNextNew);
			}
			userAll.userRatingBaseNextNewList = userRatingBaseNextNews.ToArray();
		}
		public static void UpdateHotNext(ref UserManager instance, ref UserAll userAll)
		{
			List<MU3.Client.UserRatingBaseHotNext> userRatingBaseHotNexts = new List<MU3.Client.UserRatingBaseHotNext>();
			foreach (var hotnext in instance.userRatingBaseHotNext)
			{
				MU3.Client.UserRatingBaseHotNext userRatingBaseHotNext = new MU3.Client.UserRatingBaseHotNext();
				hotnext.copyTo(userRatingBaseHotNext);
				userRatingBaseHotNexts.Add(userRatingBaseHotNext);
			}
			userAll.userRatingBaseHotNextList = userRatingBaseHotNexts.ToArray();
		}
		public static void UpdateUserMusic(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			Dictionary<int, MU3.User.UserMusic>.ValueCollection.Enumerator enumerator = instance.userMusic.Values.GetEnumerator();
			while (enumerator.MoveNext())
			{
				foreach (var userFumen in enumerator.Current.UserFumen)
				{
					if (userFumen != null && userFumen.IsNewOrModified)
					{
						UserMusicDetail userMusicDetail = new UserMusicDetail();
						userFumen.copyTo(userMusicDetail);
						buffer.add(userMusicDetail, userFumen.IsNew);
					}
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userMusicDetailList = buffer.toArray<UserMusicDetail>();
			userAll.isNewMusicDetailList = buffer.toString();
		}
		public static void UpdateUserTechCount(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			Dictionary<MU3.DB.LevelgroupID, MU3.User.UserTechCount>.ValueCollection.Enumerator enumerator2 = instance.userTechCount.Values.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				MU3.Client.UserTechCount userTechCount = new MU3.Client.UserTechCount();
				enumerator2.Current.copyTo(userTechCount);
				buffer.add(userTechCount, enumerator2.Current.IsNew);
			}
			flag |= 0 < buffer.Count;
			userAll.userTechCountList = buffer.toArray<MU3.Client.UserTechCount>();
			userAll.isNewTechCountList = buffer.toString();
		}
		public static void UpdateUserCharater(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			Dictionary<int, UserChara>.ValueCollection.Enumerator enumerator3 = instance.UserCharacter.Values.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				if (enumerator3.Current.IsNewOrModified)
				{
					UserCharacter userCharacter = new UserCharacter();
					enumerator3.Current.copyTo(userCharacter);
					buffer.add(userCharacter, enumerator3.Current.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userCharacterList = buffer.toArray<UserCharacter>();
			userAll.isNewCharacterList = buffer.toString();
		}
		public static void UpdateUserCards(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag, ref bool isLogout)
		{
			buffer.clear();
			Dictionary<int, MU3.User.UserCard>.ValueCollection.Enumerator enumerator4 = instance.UserCards.Values.GetEnumerator();
			while (enumerator4.MoveNext())
			{
				if (isLogout)
				{
					enumerator4.Current.IsNewGet = false;
				}
				if (enumerator4.Current.IsNewOrModified)
				{
					MU3.Client.UserCard userCard = new MU3.Client.UserCard();
					enumerator4.Current.copyTo(userCard);
					buffer.add(userCard, enumerator4.Current.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userCardList = buffer.toArray<MU3.Client.UserCard>();
			userAll.isNewCardList = buffer.toString();
		}
		public static void UpdateUserDeck(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			List<MU3.User.UserDeck>.Enumerator enumerator5 = instance.UserDeck.GetEnumerator();
			while (enumerator5.MoveNext())
			{
				if (enumerator5.Current.IsNewOrModified)
				{
					MU3.Client.UserDeck userDeck = new MU3.Client.UserDeck();
					enumerator5.Current.copyTo(userDeck);
					buffer.add(userDeck, enumerator5.Current.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userDeckList = buffer.toArray<MU3.Client.UserDeck>();
			userAll.isNewDeckList = buffer.toString();
		}
		public static void UpdateUserTrainingRoom(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			List<MU3.User.UserTrainingRoom>.Enumerator enumerator6 = instance.UserTrainingRoom.GetEnumerator();
			while (enumerator6.MoveNext())
			{
				if (enumerator6.Current.IsNewOrModified)
				{
					MU3.Client.UserTrainingRoom userTrainingRoom = new MU3.Client.UserTrainingRoom();
					enumerator6.Current.copyTo(userTrainingRoom);
					buffer.add(userTrainingRoom, enumerator6.Current.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userTrainingRoomList = buffer.toArray<MU3.Client.UserTrainingRoom>();
			userAll.isNewTrainingRoomList = buffer.toString();
		}
		public static void UpdateUserStory(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			Dictionary<int, MU3.User.UserStory>.ValueCollection.Enumerator enumerator7 = instance.userStory.Values.GetEnumerator();
			while (enumerator7.MoveNext())
			{
				if (enumerator7.Current.IsNewOrModified)
				{
					MU3.Client.UserStory userStory = new MU3.Client.UserStory();
					enumerator7.Current.copyTo(userStory);
					buffer.add(userStory, enumerator7.Current.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userStoryList = buffer.toArray<MU3.Client.UserStory>();
			userAll.isNewStoryList = buffer.toString();
		}
		public static void UpdateUserChapter(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			Dictionary<int, MU3.User.UserChapter>.ValueCollection.Enumerator enumerator8 = instance.UserChapter.Values.GetEnumerator();
			while (enumerator8.MoveNext())
			{
				if (enumerator8.Current.IsNewOrModified)
				{
					MU3.Client.UserChapter userChapter = new MU3.Client.UserChapter();
					enumerator8.Current.copyTo(userChapter);
					buffer.add(userChapter, enumerator8.Current.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userChapterList = buffer.toArray<MU3.Client.UserChapter>();
			userAll.isNewChapterList = buffer.toString();
		}
		public static void UpdateUserItem(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			Dictionary<int, UserPlate>.ValueCollection.Enumerator enumerator9 = instance.userPlate.Values.GetEnumerator();
			while (enumerator9.MoveNext())
			{
				if (enumerator9.Current.IsNewOrModified)
				{
					UserItem userItem = new UserItem();
					enumerator9.Current.copyTo(userItem);
					buffer.add(userItem, enumerator9.Current.IsNew);
				}
			}
			Dictionary<int, UserTrophy>.ValueCollection.Enumerator enumerator10 = instance.userTrophy.Values.GetEnumerator();
			while (enumerator10.MoveNext())
			{
				if (enumerator10.Current.IsNewOrModified)
				{
					UserItem userItem2 = new UserItem();
					enumerator10.Current.copyTo(userItem2);
					buffer.add(userItem2, enumerator10.Current.IsNew);
				}
			}
			Dictionary<int, UserPresent>.ValueCollection.Enumerator enumerator11 = instance.userPresent.Values.GetEnumerator();
			while (enumerator11.MoveNext())
			{
				if (enumerator11.Current.IsNewOrModified)
				{
					UserItem userItem3 = new UserItem();
					enumerator11.Current.copyTo(userItem3);
					buffer.add(userItem3, enumerator11.Current.IsNew);
				}
			}
			Dictionary<int, UserLimitBreakItem>.ValueCollection.Enumerator enumerator12 = instance.userLimitBreakItem.Values.GetEnumerator();
			while (enumerator12.MoveNext())
			{
				if (enumerator12.Current.IsNewOrModified)
				{
					UserItem userItem4 = new UserItem();
					enumerator12.Current.copyTo(userItem4);
					buffer.add(userItem4, enumerator12.Current.IsNew);
				}
			}
			Dictionary<int, UserProfileVoice>.ValueCollection.Enumerator enumerator13 = instance.userProfileVoice.Values.GetEnumerator();
			while (enumerator13.MoveNext())
			{
				if (enumerator13.Current.IsNewOrModified)
				{
					UserItem userItem5 = new UserItem();
					enumerator13.Current.copyTo(userItem5);
					buffer.add(userItem5, enumerator13.Current.IsNew);
				}
			}
			Dictionary<int, UserGachaTicket>.ValueCollection.Enumerator enumerator14 = instance.userGachaTicket.Values.GetEnumerator();
			while (enumerator14.MoveNext())
			{
				if (enumerator14.Current.IsNewOrModified)
				{
					UserItem userItem6 = new UserItem();
					enumerator14.Current.copyTo(userItem6);
					buffer.add(userItem6, enumerator14.Current.IsNew);
				}
			}
			foreach (UserKaikaItem item in instance.userKaikaItem)
			{
				if (item.IsNewOrModified)
				{
					UserItem userItem7 = new UserItem();
					item.copyTo(userItem7);
					buffer.add(userItem7, item.IsNew);
				}
			}
			Dictionary<int, UserExpUpItem>.ValueCollection.Enumerator enumerator16 = instance.userExpUpItem.Values.GetEnumerator();
			while (enumerator16.MoveNext())
			{
				if (enumerator16.Current.IsNewOrModified)
				{
					UserItem userItem8 = new UserItem();
					enumerator16.Current.copyTo(userItem8);
					buffer.add(userItem8, enumerator16.Current.IsNew);
				}
			}
			Dictionary<int, UserIntimateUpItem>.ValueCollection.Enumerator enumerator17 = instance.userIntimateUpItem.Values.GetEnumerator();
			while (enumerator17.MoveNext())
			{
				if (enumerator17.Current.IsNewOrModified)
				{
					UserItem userItem9 = new UserItem();
					enumerator17.Current.copyTo(userItem9);
					buffer.add(userItem9, enumerator17.Current.IsNew);
				}
			}
			Dictionary<int, UserBookItem>.ValueCollection.Enumerator enumerator18 = instance.userBookItem.Values.GetEnumerator();
			while (enumerator18.MoveNext())
			{
				if (enumerator18.Current.IsNewOrModified)
				{
					UserItem userItem10 = new UserItem();
					enumerator18.Current.copyTo(userItem10);
					buffer.add(userItem10, enumerator18.Current.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userItemList = buffer.toArray<UserItem>();
			userAll.isNewItemList = buffer.toString();
		}
		public static void UpdateUserMusicItem(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			Dictionary<int, MU3.User.UserMusic>.ValueCollection.Enumerator enumerator19 = instance.userMusic.Values.GetEnumerator();
			while (enumerator19.MoveNext())
			{
				MU3.User.UserMusicItem userMusicItem = enumerator19.Current.UserMusicItem;
				if (userMusicItem != null && userMusicItem.IsNewOrModified)
				{
					MU3.Client.UserMusicItem userMusicItem2 = new MU3.Client.UserMusicItem();
					enumerator19.Current.copyTo(userMusicItem2);
					buffer.add(userMusicItem2, userMusicItem.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userMusicItemList = buffer.toArray<MU3.Client.UserMusicItem>();
			userAll.isNewMusicItemList = buffer.toString();
		}
		public static void UpdateUserBoss(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			Dictionary<int, MU3.User.UserMusic>.ValueCollection.Enumerator enumerator20 = instance.userMusic.Values.GetEnumerator();
			while (enumerator20.MoveNext())
			{
				MU3.User.UserBoss userBoss = enumerator20.Current.UserBoss;
				if (userBoss != null && userBoss.IsNewOrModified)
				{
					MU3.Client.UserBoss userBoss2 = new MU3.Client.UserBoss();
					enumerator20.Current.copyTo(userBoss2);
					buffer.add(userBoss2, userBoss.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userBossList = buffer.toArray<MU3.Client.UserBoss>();
			userAll.isNewBossList = buffer.toString();
		}
		public static void UpdateUserLoginBonus(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			Dictionary<int, MU3.User.UserLoginBonus>.ValueCollection.Enumerator enumerator21 = instance.userLoginBonus.Values.GetEnumerator();
			while (enumerator21.MoveNext())
			{
				if (enumerator21.Current.IsNewOrModified)
				{
					MU3.Client.UserLoginBonus userLoginBonus = new MU3.Client.UserLoginBonus();
					enumerator21.Current.copyTo(userLoginBonus);
					buffer.add(userLoginBonus, enumerator21.Current.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userLoginBonusList = buffer.toArray<MU3.Client.UserLoginBonus>();
			userAll.isNewLoginBonusList = buffer.toString();
		}
		public static void UpdateUserEventPoint(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			Dictionary<int, UserRankingEvent>.ValueCollection.Enumerator enumerator22 = instance.userRankingEvent.Values.GetEnumerator();
			while (enumerator22.MoveNext())
			{
				if (enumerator22.Current.IsNewOrModified)
				{
					UserEventPoint userEventPoint = new UserEventPoint();
					enumerator22.Current.copyTo(userEventPoint);
					buffer.add(userEventPoint, enumerator22.Current.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userEventPointList = buffer.toArray<UserEventPoint>();
			userAll.isNewEventPointList = buffer.toString();
		}
		public static void UpdateUserMissionPoint(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			Dictionary<int, UserMission>.ValueCollection.Enumerator enumerator23 = instance.userMission.Values.GetEnumerator();
			while (enumerator23.MoveNext())
			{
				if (enumerator23.Current.IsNewOrModified)
				{
					UserMissionPoint userMissionPoint = new UserMissionPoint();
					enumerator23.Current.copyTo(userMissionPoint);
					buffer.add(userMissionPoint, enumerator23.Current.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userMissionPointList = buffer.toArray<UserMissionPoint>();
			userAll.isNewMissionPointList = buffer.toString();
		}
		public static void UpdateUserRatingLogs(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			Dictionary<uint, MU3.User.UserRatinglog>.ValueCollection.Enumerator enumerator24 = instance.userRatinglog.Values.GetEnumerator();
			while (enumerator24.MoveNext())
			{
				if (enumerator24.Current.IsNewOrModified)
				{
					MU3.Client.UserRatinglog userRatinglog = new MU3.Client.UserRatinglog();
					enumerator24.Current.copyTo(userRatinglog);
					buffer.add(userRatinglog, enumerator24.Current.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userRatinglogList = buffer.toArray<MU3.Client.UserRatinglog>();
			userAll.isNewRatinglogList = buffer.toString();
		}
		public static void UpdateUserScnario(ref UserManager instance, ref UserAll userAll, ref Buffer buffer, ref bool flag)
		{
			buffer.clear();
			Dictionary<int, MU3.User.UserScenario>.ValueCollection.Enumerator enumerator25 = instance.userScenario.Values.GetEnumerator();
			while (enumerator25.MoveNext())
			{
				if (enumerator25.Current.IsNewOrModified)
				{
					MU3.Client.UserScenario userScenario = new MU3.Client.UserScenario();
					enumerator25.Current.copyTo(userScenario);
					buffer.add(userScenario, enumerator25.Current.IsNew);
				}
			}
			flag |= 0 < buffer.Count;
			userAll.userScenarioList = buffer.toArray<MU3.Client.UserScenario>();
			userAll.isNewScenarioList = buffer.toString();
		}

		private enum State
		{
			FadeIn,
			LevelUp,
			Upsert,
			LogOut,
			GameOver,
			End
		}

		/*[MethodPatch(PatchType.Prefix, typeof(MU3.Scene_39_Logout), "Upsert_Init")]
		public static bool UpsertInit(ref MU3.Scene_39_Logout __instance)
        {
			var mode_Accessor = typeof(MU3.Scene_39_Logout).GetField("mode_", (BindingFlags)62);
			//var packet_Accessor = typeof(MU3.Scene_39_Logout).GetField("packet_", (BindingFlags)62);
			if (SingletonStateMachine<AMManager, AMManager.EState>.instance.backup.setting.isEventModeSettingAvailable)
			{
				((Mode<MU3.Scene_39_Logout, State>)mode_Accessor.GetValue(__instance)).set(State.LogOut);
				return false;
			}
			//PacketUpsertUserAll packetUpsertUserAll = new PacketUpsertUserAll();
			bool result = false;
			//PacketUpsertUserAllCreate(ref result, ref packetUpsertUserAll, true, true);
			//packetUpsertUserAll.create(firstTime: true, isLogout: true)
			if (!result)
			{
				((Mode<MU3.Scene_39_Logout, State>)mode_Accessor.GetValue(__instance)).set(State.LogOut);
			}
			//packet_Accessor.SetValue(__instance, packetUpsertUserAll);
			return false;
		}*/

		/*[MethodPatch(PatchType.Prefix, typeof(PacketUpsertUserAll), "create", new Type[] { typeof(bool), typeof(bool) })]
		public static bool PacketUpsertUserAllCreate(ref bool __result, ref PacketUpsertUserAll __instance, bool firstTime, bool isLogout)
        {
			Buffer buffer = new Buffer();
			bool flag = firstTime;
			UpsertUserAll upsertUserAll = new UpsertUserAll();
			UserManager instance = Singleton<UserManager>.instance;
			upsertUserAll.request_.userId = instance.UserId;
			UserAll userAll = new UserAll();
			
			UpdateUserDetail(ref instance, ref userAll);
			UpdatePlaylogList(ref instance, ref userAll, ref flag);
			UpdateSessionlogList(ref instance, ref userAll, ref isLogout);
			if (!instance.IsGuest)
			{
				UpdateUserActivityList(ref instance, ref userAll);
				UpdateUserRecentRating(ref instance, ref userAll);
				UpdateUserBpBase(ref instance, ref userAll);
				UpdateBestNew(ref instance, ref userAll);
				UpdateBest(ref instance, ref userAll);
				UpdateHot(ref instance, ref userAll);
				UpdateNext(ref instance, ref userAll);
				UpdateNextNew(ref instance, ref userAll);
				UpdateHotNext(ref instance, ref userAll);

				UpdateUserMusic(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserTechCount(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserCharater(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserCards(ref instance, ref userAll, ref buffer, ref flag, ref isLogout);
				UpdateUserDeck(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserTrainingRoom(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserStory(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserChapter(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserItem(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserMusicItem(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserBoss(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserLoginBonus(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserEventPoint(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserMissionPoint(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserRatingLogs(ref instance, ref userAll, ref buffer, ref flag);
				UpdateUserScnario(ref instance, ref userAll, ref buffer, ref flag);
			}
			if (!flag)
			{
				__result = false;
				return false;
			}
			upsertUserAll.request_.upsertUserAll = userAll;
			typeof(Packet).GetMethod("create", (BindingFlags)62).Invoke(__instance, new object[1] { upsertUserAll });
			__result = flag;
			return false;
		}*/
	}
}
